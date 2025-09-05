
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Order;
using Plaza.Net.IServices.Sys;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plaza.Net.MVCAdmin.Controllers.Order
{
    public class PaymentRecordController : Controller
    {
        private readonly IPaymentRecordService _paymentRecordService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly EFDbContext _dbContext;

        public PaymentRecordController(
            IPaymentRecordService paymentRecordService,
            IOrderService orderService,
            IUserService userService,
            EFDbContext dbContext)
        {
            _paymentRecordService = paymentRecordService;
            _orderService = orderService;
            _userService = userService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? orderId)
        {
            // 获取支付方式字典项
            var paymentMethods = await _dbContext.Dictionary
                .Where(d => d.Name == "支付方式")
                .Include(di => di.DictionaryItems)
                .SelectMany(di => di.DictionaryItems)
                .ToListAsync();

            // 获取支付状态字典项
            var paymentStatus = await _dbContext.Dictionary
                .Where(d => d.Name == "支付状态")
                .Include(di => di.DictionaryItems)
                .SelectMany(di => di.DictionaryItems)
                .ToListAsync();

            // 获取订单列表
            var orders = await _orderService.GetAllAsync();

            // 获取用户列表
            var users = await _userService.GetAllAsync();

            var viewModel = new PaymentRecordViewModel
            {
                PaymentMethods = paymentMethods,
                PaymentStatus = paymentStatus,
                Orders = orders,
                Users = users
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentRecordsData(
            int pageIndex,
            int pageSize,
            int? paymentMethodId,
            int? paymentStatusId,
            int? orderId,
            int? userId,
            DateTime? startTime,
            DateTime? endTime,
            string keyword = null!)
        {
            Expression<Func<PaymentRecordEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.TransactionId.Contains(keyword) ||
                 p.Order.Code.Contains(keyword)) &&
                (!paymentMethodId.HasValue || p.PaymentMethodItemId == paymentMethodId.Value) &&
                (!paymentStatusId.HasValue || p.PaystatuItemId == paymentStatusId.Value) &&
                (!orderId.HasValue || p.OrderId == orderId.Value) &&
                (!userId.HasValue || p.UserId == userId.Value) &&
                (!startTime.HasValue || p.PaymentTime >= startTime.Value) &&
                (!endTime.HasValue || p.PaymentTime <= endTime.Value);

            var query = await _paymentRecordService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(pr => pr.Order)
                              .Include(pr => pr.User)
                              .Include(pr => pr.PaymentMethodItem)
                              .Include(pr => pr.PaystatuItem));

            var total = await _paymentRecordService.CountByAsync(predicate);

            var result = query.Select(pr => new
            {
                pr.Id,
                pr.PaymentMethodItemId,
                PaymentMethod = pr.PaymentMethodItem.Label,
                pr.Amount,
                pr.PaymentTime,
                pr.PaystatuItemId,
                PaymentStatus = pr.PaystatuItem.Label,
                pr.TransactionId,
                pr.OrderId,
                OrderCode = pr.Order.Code,
                pr.UserId,
                userName = pr.User.UserName,
                pr.CreateTime,
                pr.UpdateTime
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(PaymentRecordDTO paymentDto)
        {
            try
            {
                if (paymentDto == null)
                {
                    return BadRequest("支付记录不能为空");
                }

                var order = await _orderService.GetOneByIdAsync(paymentDto.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "订单不存在" });
                }

                var user = await _userService.GetOneByIdAsync(paymentDto.UserId);
                if (user == null)
                {
                    return Json(new { success = false, message = "用户不存在" });
                }

                var paymentRecord = new PaymentRecordEntity
                {
                    PaymentMethodItemId = paymentDto.PaymentMethodItemId,
                    Amount = paymentDto.Amount,
                    PaymentTime = paymentDto.PaymentTime,
                    PaystatuItemId = paymentDto.PaystatuItemId,
                    TransactionId = paymentDto.TransactionId,
                    OrderId = paymentDto.OrderId,
                    UserId = paymentDto.UserId,
                };

                var result = await _paymentRecordService.CreateAsync(paymentRecord);

                if (result)
                {
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
        public async Task<IActionResult> Edit(PaymentRecordDTO paymentDto)
        {
            try
            {
                var originalPayment = await _paymentRecordService.GetOneByIdAsync(paymentDto.Id);
                if (originalPayment == null)
                {
                    return Json(new { success = false, message = "支付记录不存在" });
                }

                originalPayment.PaymentMethodItemId = paymentDto.PaymentMethodItemId;
                originalPayment.Amount = paymentDto.Amount;
                originalPayment.PaymentTime = paymentDto.PaymentTime;
                originalPayment.PaystatuItemId = paymentDto.PaystatuItemId;
                originalPayment.TransactionId = paymentDto.TransactionId;
                originalPayment.OrderId = paymentDto.OrderId;
                originalPayment.UserId = paymentDto.UserId;
                originalPayment.UpdateTime = DateTime.Now;

                var result = await _paymentRecordService.UpdateAsync(originalPayment);

                if (result)
                {
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
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _paymentRecordService.DeleteRangeAsync(ids);

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
