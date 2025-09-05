using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Plaza.Net.IServices.Order;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Model.ViewModels.Pay;
using Plaza.Net.Services.Order;
using Plaza.Net.Utility.Helper;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;



namespace Plaza.Net.WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IAopClient _alipay;
        private readonly IOptions<AlipayOptions> _opt;
        private readonly ILogger<OrderEntity> _logger;
        private readonly IPaymentRecordService _paymentRecordService;
        private readonly IReviewService _reviewService;

        public OrderController(IDictionaryService dictionaryService,
            IDictionaryItemService dictionaryItemService,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IAopClient alipay,
            IOptions<AlipayOptions> opt,
            ILogger<OrderEntity> logger,
            IPaymentRecordService paymentRecordService,
            IReviewService reviewService)
        {
            _dictionaryService = dictionaryService;
            _dictionaryItemService = dictionaryItemService;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _alipay = alipay;
            _opt = opt;
            _logger = logger;
            _paymentRecordService = paymentRecordService;
            _reviewService = reviewService;
        }
        [HttpPost("submitorder")]
        public async Task<IActionResult> SubmitOrder(SubmitOrderDTO dto)
        {
            if (dto == null)
                return BadRequest("请求体为空");

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Items 为空");
            if (!ModelState.IsValid)
            {
                // 把所有验证错误拼成字符串返回
                var errs = string.Join(" | ",
                    ModelState.SelectMany(m => m.Value!.Errors)
                              .Select(e => e.ErrorMessage));
                return BadRequest(errs);          // 400，内容就是字段错误
               
            }
            dto.OrderStatuItemId = 30;
            dto.Code = $"SN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
            #region 构造实体
            var entity = new OrderEntity
            {
                CustomerId = dto.CustomerId,
                StoreId = dto.StoreId,
                DeliveryType = dto.DeliveryType,
                TotalAmount = dto.Items.Sum(i => i.Price * i.Quantity),
                OrderStatuItemId = dto.OrderStatuItemId,        // 待支付
                Code =dto.Code,

                ShippingAddress = dto.DeliveryType == 1
                                  ? System.Text.Json.JsonSerializer.Serialize(dto.Address)
                                  : null,
                PickUpDate = dto.DeliveryType == 0 ? dto.PickupDate : null,
                // ↑↑↑ 新增 ↑↑↑
              
                Items = dto.Items.Select(i => new OrderItemEntity
                {
                    ProductSkuId = i.SkuId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price
                }).ToList()
            };
            #endregion

            var order = await _orderService.CreateAsync(entity);

            return Ok(new
            {
                Code = entity.Code,
                TotalAmount = entity.TotalAmount,
                CustomerId = entity.CustomerId ,
                StoreId = entity.StoreId,
                DeliveryType = entity.DeliveryType,
                
                ShippingAddress =entity.ShippingAddress,
                PickUpDate = entity.DeliveryType
            });
        }
        [HttpPost("pay")]
        public async Task<IActionResult> Pay(string orderNo)
        {
          
            if (string.IsNullOrWhiteSpace(orderNo))
                return BadRequest("订单号不能为空");

            var order = await _orderService.GetOneByCodeAsync(orderNo);
            if (order == null)
                return NotFound(new { code = 404, message = "订单不存在" });
            var ua = Request.Headers["User-Agent"].ToString();
            bool isMobile = Regex.IsMatch(ua, @"Mobile|Android|iPhone|iPad", RegexOptions.IgnoreCase);
            string productCode = isMobile ? "QUICK_WAP_WAY" : "FAST_INSTANT_TRADE_PAY";
            var request = new AlipayTradePagePayRequest();
            request.BizContent = JsonConvert.SerializeObject(new
            {
                out_trade_no = order.Code,
                total_amount = order.TotalAmount.ToString("0.00"),
                subject = order.Store.Name + "订单"  ,
                product_code = productCode,
                ReturnUrl = $"{Request.Scheme}://{Request.Host}/api/order/return",   
                NotifyUrl = $"{Request.Scheme}://{Request.Host}/api/order/notify",
            });
            Console.WriteLine(request.BizContent);
            string html = _alipay.pageExecute(request, null, "POST").Body;
            return Content(html, "text/html");
        }
        [HttpGet("return")]
        public IActionResult Return()
        {
            // 1. 读取支付宝回参（过滤空值）
            var dict = Request.Query
                              .Where(kv => !string.IsNullOrEmpty(kv.Value))
                              .ToDictionary(kv => kv.Key, kv => kv.Value!);

            // 2. 必须有 sign
            if (!dict.ContainsKey("sign"))
            {
                _logger.LogWarning("缺少 sign，直接失败");
                return Redirect("http://localhost:5173/#/pages/order/result?status=fail");
            }

            // 3. 验签
            bool signOk = AlipaySignature.RSACheckV1((IDictionary<string, string>)dict,
                                                     _opt.Value.AlipayPublicKey,
                                                     _opt.Value.Charset,
                                                     _opt.Value.SignType,
                                                     false);

            // 4. 根据交易状态决定展示
            string status = "fail";
            if (signOk)
            {
                var ts = dict.GetValueOrDefault("trade_status");
                if (ts == "TRADE_SUCCESS" || ts == "TRADE_FINISHED")
                    status = "success";
            }

            // 5. 跳转到 uni-app 结果页（不带任何敏感参数）
            string frontUrl = $"http://localhost:5173/#/pages/order/result?status={status}";
            return Redirect(frontUrl);
        }
        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            // 1. 兼容读取：先判断是否有表单，否则手动解析原始流
            Dictionary<string, string> dict = new();
            if (Request.HasFormContentType)
            {
                foreach (var k in Request.Form.Keys)
                    dict[k] = Request.Form[k];
            }
            else
            {
                // 兜底：手动解析 application/x-www-form-urlencoded 原始流
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    foreach (var kv in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var pair = kv.Split('=', 2);
                        if (pair.Length == 2)
                            dict[pair[0]] = Uri.UnescapeDataString(pair[1]);
                    }
                }
            }

            // 2. 验签（必须有 sign）
            if (!dict.ContainsKey("sign") ||
                !AlipaySignature.RSACheckV1(dict, _opt.Value.AlipayPublicKey,
                                            _opt.Value.Charset, _opt.Value.SignType, false))
            {
                _logger.LogWarning("支付宝通知验签失败");
                return Content("fail", "text/plain");
            }

            // 3. 业务逻辑（同你原来代码）
            var tradeStatus = dict.GetValueOrDefault("trade_status");
            if (tradeStatus != "TRADE_SUCCESS" && tradeStatus != "TRADE_FINISHED")
                return Content("success", "text/plain");

            var outTradeNo = dict["out_trade_no"];
            var tradeNo = dict["trade_no"];
            if (!decimal.TryParse(dict["total_amount"], out var totalAmount))
                return Content("fail", "text/plain");

            var order = await _orderService.GetOneByCodeAsync(outTradeNo);
            if (order == null || order.OrderStatuItemId != 75)
                return Content("success", "text/plain");

            order.OrderStatuItemId = 76;   // 已支付
            order.UpdateTime = DateTime.Now;

            await _paymentRecordService.CreateAsync(new PaymentRecordEntity
            {
                OrderId = order.Id,
                UserId = order.CustomerId,
                PaymentMethodItemId = 39,
                PaystatuItemId = 32,
                Amount = totalAmount,
                PaymentTime = DateTime.Now,
                TransactionId = tradeNo
            });
            await _orderService.UpdateAsync(order);

            return Content("success", "text/plain");
        }
        [HttpGet("orderStatu")]
        public async Task<IActionResult> OrderStatus()
        {
            var orderStatus = await _dictionaryService.GetOneByCodeAsync("OrderStatus");
            if (orderStatus == null)
            {
                return NotFound("未找到订单状态字典");
            }
            var item = await _dictionaryItemService.GetManyByAsync(d => d.DictionaryId == orderStatus.Id);


            if (item == null) return NotFound("未找到订单状态项");
            var result = item.Select(item => new
            {
                Id = item.Id,
                Name = item.Label,
            });
            return Ok(result);
        }
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders
            (
                int customerId,
                int? plazaId,
                int? storeId,
                int? statusId,
                int pageIndex = 1,
                int pageSize = 10)
        {
            Expression<Func<OrderEntity, bool>> predicate = p =>
              (p.Customer != null && p.CustomerId == customerId) &&
              (!plazaId.HasValue || p.Store.Floor.Plaza.Id == plazaId.Value)&&
                (!storeId.HasValue || p.StoreId == storeId.Value) &&
              (!statusId.HasValue || p.OrderStatuItemId == statusId.Value);


            var order = await _orderService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(o => o.Store)
                .Include(o => o.OrderStatuItem)
                .Include(o => o.Items)
                .ThenInclude(i => i.ProductSku)
                .ThenInclude(s => s.Product)
                .Include(o => o.Items)
                .ThenInclude(i => i.ProductSku)
                .ThenInclude(s => s.SpecValueMappings)
                .ThenInclude(m => m.ProductSpecValue));
            var result = order.Select(o => new OrderDTO
            {
                Id = o.Id,
                StoreName = o.Store.Name,
                PlazaName = o.Store.Floor.Plaza.Name,
                OrderStatus = o.OrderStatuItem.Label,
                TotalAmount = o.TotalAmount,
                DeliveryType = o.DeliveryType,
                GoodsCount = o.Items.Count,
                Items = o.Items.Select(i => new OrderItemDTO
                {
                    SkuId = i.ProductSkuId,
                    Title = i.ProductSku.Product.Name,
                    Spec = string.Join("/",
            i.ProductSku.SpecValueMappings
                        .Select(s => s.ProductSpecValue.Value)),
                    ImageUrl = i.ProductSku.ImageUrl,
                    Price = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            });
            return Ok(result);

        }
        [HttpGet("orderitem")]
        public async Task<IActionResult> OrderItems(int id)
        {
            var order = await _orderService.GetOneByIdAsync(id);
            if (order == null)
            {
                return NotFound("订单不存在");
            }
            if (order.OrderStatuItem.Label != "已完成")
            {
                return BadRequest("当前状态不能评价");
            }
            var dto = new
            {
                orderId = order.Id,
                orderNo = order.Code,
                storeName = order.Store.Name,
                totalAmount = order.TotalAmount,
                orderStatus = order.OrderStatuItem.Label,
                items = order.Items.Select(i => new
                {
                    itemId = i.Id,          // 明细主键，评价提交用
                    skuId = i.ProductSkuId,
                    title = i.ProductSku.Product.Name,
                    spec = string.Join("/", i.ProductSku.SpecValueMappings
                                                 .Select(m => m.ProductSpecValue.Value)),
                    imageUrl = i.ProductSku.ImageUrl ?? "/static/logo.png",
                    price = i.UnitPrice,
                    quantity = i.Quantity
                }).ToList()
            };
          
            return Ok(dto);
        }
        [HttpPost("reviews")]
        public async Task<IActionResult> SubmitReview([FromBody] List<ReviewSubmitDTO> dtoList)
        {


            if (dtoList == null || !dtoList.Any())
                return BadRequest("缺少评价数据");

            var results = new List<object>();

            foreach (var dto in dtoList)
            {
                // 1. 查明细
                var orderItem = await _orderItemService.GetOneByIdAsync(dto.ItemId);
                if (orderItem == null)
                {
                    results.Add(new { itemId = dto.ItemId, ok = false, msg = "订单明细不存在" });
                    continue;
                }

                // 2. 校验订单状态
                var order = await _orderService.GetOneByIdAsync(orderItem.OrderId);
                if (order?.OrderStatuItem.Label != "已完成")
                {
                    results.Add(new { itemId = dto.ItemId, ok = false, msg = "当前状态不能评价" });
                    continue;
                }

                // 3. 入库
                var entity = new ReviewEntity
                {
                    OrderItemId = dto.ItemId,
                    ReviewRatingItemId = dto.Rate,
                    Content = dto.Content?.Trim() ?? "",
                    ImageUrl = dto.ImageUrls?.Length > 1000 ? dto.ImageUrls[..1000] : dto.ImageUrls,
                    UserId = dto.UserId,
                };
                var ok = await _reviewService.CreateAsync(entity);
              
               
                results.Add(new { itemId = dto.ItemId, ok, msg = ok ? "评论成功" : "评论失败" });
            }

            return Ok(new { code = 0, data = results });

        }
        // 取消订单（示例实现）
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderService.GetOneByIdAsync(id);
            if (order.OrderStatuItemId != 75) // 75=待支付
                return BadRequest("只有待支付订单可以取消");

            order.OrderStatuItemId = 77; // 77=已取消
            await _orderService.UpdateAsync(order);
            return Ok();
        }
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmReceive(int id)
        {
            var order = await _orderService.GetOneByIdAsync(id);
            if (order.OrderStatuItemId != 78) // 78=已发货
                return BadRequest("只有已发货订单可以确认收货");

            order.OrderStatuItemId = 79; // 79=已完成
            order.UpdateTime = DateTime.Now;
            await _orderService.UpdateAsync(order);
            return Ok();
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrder(int id, [FromBody] OrderDelDTO dto)
        {
            var order = await _orderService.GetOneByIdAsync(id);
            if (order == null)
                return NotFound("订单不存在");
            if (order.OrderStatuItemId == 75 || order.OrderStatuItemId == 79 || order.OrderStatuItemId == 80 || order.OrderStatuItemId == 82)
            {
                dto.IsDeleted = true;

                order.IsDeleted = dto.IsDeleted;
                
                order.UpdateTime = DateTime.Now;
                var result=await _orderService.UpdateAsync(order);
                return Ok(result);
            }

            return NotFound();
          
           
        }
    }
}
