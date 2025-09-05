using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;

using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Sys
{
    public class LoginLogController : Controller
    {
        private readonly ILoginLogService _loginLogService;
        public LoginLogController(ILoginLogService loginLogService)
        {
            _loginLogService = loginLogService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetData(int pageIndex, int pageSize, int? status, string keyword = null!, string userName = null!)
        {
            // 组合查询条件
            Expression<Func<LoginLogEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.User.UserName.Contains(keyword) ||
                 p.Code.Contains(keyword) ||
                 p.User != null && p.User.UserName.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(userName) || p.User != null && p.User.UserName.Contains(userName)) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1));

            var query = await _loginLogService.GetPagedListByAsync(pageIndex, pageSize, predicate, include: p => p.Include(f => f.User));
            var Total = await _loginLogService.CountByAsync(predicate);

            // 转换结果，确保返回广场名称而不是ID
            var result = query.Select(f => new
            {
                f.Id,
                f.DeviceInfo,
                f.FailureReason,
                f.Status,
                f.UserId,
                f.Code,
                f.LoginTime,
                f.LogoutTime,
                f.IsDeleted,
                f.CreateTime,
                f.UpdateTime,
                userName = f.User.UserName
                // 使用null条件操作符处理可能的null Plaza
            });

            return Json(new
            {
                rows = result,
                total = Total,
            });
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

                var result = await _loginLogService.DeleteRangeAsync(ids);

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
