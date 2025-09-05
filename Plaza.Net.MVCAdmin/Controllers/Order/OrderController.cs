using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Order;
using Plaza.Net.IServices.Store;
using Plaza.Net.IServices.Sys;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels;
using Plaza.Net.Model.ViewModels.DTO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plaza.Net.MVCAdmin.Controllers.Order
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly IFloorService _floorService;
        private readonly IPlazaService _plazaService;
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;
        private readonly IOrderItemService _orderItemService;
        private readonly EFDbContext _dbContext;

        public OrderController(
            IOrderService orderService,
            IProductService productService,
            IStoreService storeService,
            IFloorService floorService,
            IPlazaService plazaService,
            IEmployeeService employeeService,
            IUserService userService,
            IOrderItemService orderItemService,
            EFDbContext dbContext)
        {
            _orderService = orderService;
            _productService = productService;
            _storeService = storeService;
            _floorService = floorService;
            _plazaService = plazaService;
            _employeeService = employeeService;
            _userService = userService;
            _orderItemService = orderItemService;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetFloorsByPlazaId(int plazaId)
        {
            var floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId);
            return Json(floors);
        }
        [HttpGet]
        public async Task<IActionResult> GetStoresByFloorId( int floorId)
        {
            try
            {
                var stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId );
                return Json(stores);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByFStoreId(int storeId)
        {
            try
            {
                var stores = await _employeeService.GetManyByAsync(e=>e.StoreId==storeId);
                return Json(stores);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        public async Task<IActionResult> GetProductByStoreId(int storeId)
        {
            try
            {
                var products = await _productService.GetManyByAsync(e => e.StoreId == storeId);
                return Json(products);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId, int? floorId, int? storeId)
        {

            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<StoreEntity> stores = new List<StoreEntity>();
            IEnumerable<EmployeeEntity> employees = new List<EmployeeEntity>();

            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
                if (floorId.GetValueOrDefault() != 0)
                {
                    stores = await _storeService.GetManyByAsync(f => f.FloorId == floorId!.Value);
                    if (storeId.GetValueOrDefault() != 0)
                    {
                        employees=await _employeeService.GetManyByAsync(f => f.StoreId == storeId!.Value);
                    }
                }
            }
           
            // 获取订单状态字典项
            var orderStatusItems = await _dbContext.Dictionary
                    .Where(d => d.Name == "订单状态")
                    .Include(di => di.DictionaryItems)
                    .SelectMany(di => di.DictionaryItems)
                    .ToListAsync();

            var viewModel = new OrderIndexViewModel
            {
                OrderStatusItems = orderStatusItems,
                Stores = stores,
                Floors = floors,
                Plazas = plazas,
                Employees= employees
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Item(int id)
        {

            // 获取订单信息
            var order = await _orderService.GetOneByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // 获取订单项
            var orderItems = await _orderItemService.GetManyByAsync(item => item.OrderId == id);

            // 获取当前商店中的产品列表
            var products = await _productService.GetManyByAsync(p => p.StoreId == order.StoreId);

            // 创建一个视图模型来传递数据
            var viewModel = new OrderItemViewModel
            {
                Order = order,
                OrderItems = orderItems,
                Products = products,
                StoreId=order.StoreId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersData(
            int pageIndex,
            int pageSize,
            int? orderStatusId,
            int? productId,
            int? employeeId,
            int? plazaId,
            int? floorId,
            int? storeId,
            DateTime? startTime,
            DateTime? endTime,
            string keyword = null!,
            string customer = null!)
        {
            // 组合查询条件
            Expression<Func<OrderEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Id.ToString().Contains(keyword) ||
                 p.Customer.UserName.Contains(keyword) ||
                
                 p.ShippingAddress.Contains(keyword)) &&
                (!orderStatusId.HasValue || p.OrderStatuItemId == orderStatusId.Value) &&
                (!plazaId.HasValue || p.Store.Floor.PlazaId == plazaId.Value) &&
                (!floorId.HasValue || p.Store.FloorId == floorId.Value) &&
                (!storeId.HasValue || p.StoreId == storeId.Value) &&
                (!employeeId.HasValue || p.EmployeeId == employeeId.Value) &&
                 (string.IsNullOrWhiteSpace(customer) || (p.Customer != null && p.Customer.UserName.Contains(customer))) &&
                (!startTime.HasValue || p.CreateTime >= startTime.Value) &&
                (!endTime.HasValue || p.CreateTime <= endTime.Value);

            var query = await _orderService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(o => o.Customer)
                              .Include(o => o.Store)
                              .ThenInclude(o => o.Floor)
                              .ThenInclude(o => o.Plaza)
                              .Include(o => o.Employee)
                              .Include(o => o.OrderStatuItem));

            var total = await _orderService.CountByAsync(predicate);

            // 转换结果
            var result = query.Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.OrderStatuItemId,
                OrderStatus = o.OrderStatuItem.Label,
                o.ShippingAddress,
                o.CustomerId,
                CustomerName = o.Customer.UserName,
                o.EmployeeId,
                EmployeeName = o.Employee?.Name ?? "未分配",
                 o.Store.Floor.PlazaId,
                PlazaName = o.Store.Floor.Plaza.Name,
                o.Store.FloorId,
                FloorName = o.Store.Floor.Name,
                o.StoreId,
                StoreName = o.Store.Name,
                o.Code,
                o.IsDeleted,
                o.CreateTime,
                o.UpdateTime
            });

            return Json(new
            {
                rows = result,
                Total = total,
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetTotalAmount(int orderId)
        {
            try
            {
                var total = await _orderItemService.GetTotalAmountAsync(orderId);
                return Json(new { success = true, totalAmount = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        public string GenerateOrderCode()
        {
            // 获取当前时间，格式化为 yyyyMMddHHmmss
            var timePart = DateTime.Now.ToString("yyyyMMddHHmmss");

            // 生成一个随机数，范围是 1000 到 9999
            var random = new Random();
            var randomPart = random.Next(1000, 9999).ToString();

            // 组合时间和随机数生成订单编号
            return $"ORD{timePart}{randomPart}";
        }
        //[HttpPost]
        //public async Task<IActionResult> Add(OrderDTO orderDto)
        //{
        //    try
        //    {
        //        if (orderDto == null)
        //        {
        //            return BadRequest("订单不能为空");
        //        }
        //        var customer = await _dbContext.Users
        //    .Where(u => u.UserName == orderDto.CustomerName)
        //    .FirstOrDefaultAsync();
        //        if (customer == null)
        //        {
        //            return Json(new { success = false, message = "用户不存在" });
        //        }
        //        orderDto.CustomerId = customer!.Id;
        //        orderDto.Code = GenerateOrderCode();
        //        decimal totalAmount = 0;
        //        if (orderDto.Items != null && orderDto.Items.Any())
        //        {
        //            totalAmount = orderDto.Items.Sum(item => item.Quantity * item.Price);
        //        }
        //        var order = new OrderEntity
        //        {
        //            TotalAmount = orderDto.TotalAmount,
        //            OrderStatuItemId = orderDto.OrderStatuItemId,
        //            ShippingAddress = orderDto.ShippingAddress!,
        //            StoreId = orderDto.StoreId,
        //            CustomerId = orderDto.CustomerId,
        //            EmployeeId = orderDto.EmployeeId,
        //            Code = orderDto.Code,
        //        };
        //        var result = await _orderService.CreateAsync(order);
        //        if (result && orderDto.Items != null && orderDto.Items.Any())
        //        {
        //            foreach (var item in orderDto.Items)
        //            {
        //                item.OrderId = order.Id;
        //                await _orderItemService.CreateAsync(item);
        //            }

        //            // 更新订单总金额
        //            var updatedTotal = await _orderItemService.GetTotalAmountAsync(order.Id);
        //            order.TotalAmount = updatedTotal;
        //            await _orderService.UpdateAsync(order);
        //        }
        //        if (result)
        //        {
        //            return Json(new { success = true, message = "添加成功" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "添加失败" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = "服务器内部错误" });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(OrderDTO orderDto)
        //{
        //    try
        //    {
        //        // 获取原始订单信息
        //        var originalOrder = await _orderService.GetOneByIdAsync(orderDto.Id);
        //        if (originalOrder == null)
        //        {
        //            return Json(new { success = false, message = "订单不存在" });
        //        }

        //        // 更新订单属性
        //        originalOrder.TotalAmount = orderDto.TotalAmount;
        //        originalOrder.OrderStatuItemId = orderDto.OrderStatuItemId;
        //        originalOrder.ShippingAddress = orderDto.ShippingAddress!;
        //        originalOrder.StoreId = orderDto.StoreId;
        //        originalOrder.EmployeeId = orderDto.EmployeeId;
        //        originalOrder.UpdateTime = DateTime.Now;

        //        // 保存订单更改
        //        var result = await _orderService.UpdateAsync(originalOrder);

        //        if (result && orderDto.Items != null && orderDto.Items.Any())
        //        {
        //            // 先删除现有订单项
        //            await _orderItemService.DeleteRangeByOrderIdAsync(originalOrder.Id);

        //            // 添加新订单项
        //            foreach (var item in orderDto.Items)
        //            {
        //                item.OrderId = originalOrder.Id;
        //                await _orderItemService.CreateAsync(item);
        //            }

        //            // 更新订单总金额
        //            var updatedTotal = await _orderItemService.GetTotalAmountAsync(originalOrder.Id);
        //            originalOrder.TotalAmount = updatedTotal;
        //            await _orderService.UpdateAsync(originalOrder);
        //        }

        //        if (result)
        //        {
        //            return Json(new { success = true, message = "更新成功" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "更新失败" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = "服务器内部错误" });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _orderService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetData(
            int orderId,
            int pageIndex,
            int pageSize,
            string keyword = null!)
        {
            Expression<Func<OrderItemEntity, bool>> predicate = p =>
                p.OrderId == orderId &&
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.ProductSku.Name.Contains(keyword) ||
                 p.ProductSku.Code.Contains(keyword));

            var query = await _orderItemService.GetItemPagedListByAsync(
                orderId,
                pageIndex,
                pageSize,
                predicate,
                include:item => item
                .Include(o => o.Order)
                .Include(o => o.ProductSku)
                .ThenInclude(o=>o.Product)
                .ThenInclude(o=>o.Store)
                .ThenInclude(o=>o.Floor)
                .ThenInclude(o=>o.Plaza)
                );
            foreach (var item in query)
            {
                Console.WriteLine($"OrderItem ID: {item.Id}, Product: {item.ProductSkuId}");
            }
            var total = await _orderItemService.CountByAsync(predicate);
            
            var result = query.Select(item => new
            {
                item.Id,
                item.ProductSkuId,
                item.OrderId,
                productName = item.ProductSku?.Name ?? "未知产品", 
                productCode = item.ProductSku?.Code ?? "未知编码",
                item.Quantity,
                item.UnitPrice,
                totalPrice = item.Quantity * item.UnitPrice
            });
            return Json(new
            {
                rows = result,
                Total = total
            });
        }



        [HttpPost]
        public async Task<IActionResult> AddItem(OrderItemEntity orderItem)
        {
            try
            {
                if (orderItem == null)
                {
                    return BadRequest("订单项不能为空");
                }

                // 验证订单和产品是否存在
                var orderExists = await _orderService.GetOneByIdAsync(orderItem.OrderId) != null;
                var productExists = await _productService.GetOneByIdAsync(orderItem.ProductSkuId) != null;

                if (!orderExists)
                {
                    return Json(new { success = false, message = "订单不存在" });
                }

                if (!productExists)
                {
                    return Json(new { success = false, message = "产品不存在" });
                }

                var result = await _orderItemService.CreateAsync(orderItem);

                if (result)
                {
                    // 更新订单总金额
                    var updatedTotal = await _orderItemService.GetTotalAmountAsync(orderItem.OrderId);
                    var order = await _orderService.GetOneByIdAsync(orderItem.OrderId);
                    if (order != null)
                    {
                        order.TotalAmount = updatedTotal;
                        await _orderService.UpdateAsync(order);
                    }

                    return Json(new { success = true, message = "添加成功" });
                }
                else
                {
                    return Json(new { success = false, message = "添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditItem(OrderItemEntity orderItem)
        {
            try
            {
                var originalOrderItem = await _orderItemService.GetOneByIdAsync(orderItem.Id);
                if (originalOrderItem == null)
                {
                    return Json(new { success = false, message = "订单项不存在" });
                }

                originalOrderItem.Quantity = orderItem.Quantity;
                originalOrderItem.UnitPrice = orderItem.UnitPrice;
                originalOrderItem.ProductSkuId = orderItem.ProductSkuId;

                var result = await _orderItemService.UpdateAsync(originalOrderItem);

                if (result)
                {
                    // 更新订单总金额
                    var updatedTotal = await _orderItemService.GetTotalAmountAsync(originalOrderItem.OrderId);
                    var order = await _orderService.GetOneByIdAsync(originalOrderItem.OrderId);
                    if (order != null)
                    {
                        order.TotalAmount = updatedTotal;
                        await _orderService.UpdateAsync(order);
                    }

                    return Json(new { success = true, message = "更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRangeItem(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _orderItemService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

    }
}
