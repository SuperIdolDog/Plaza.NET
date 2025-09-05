using Microsoft.AspNetCore.Mvc;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model.Entities.Basic;
using System.Drawing.Printing;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Plaza.Net.MVCAdmin.Controllers.Basic
{
    public class PlazaController : Controller
    {
        private readonly ILogger<PlazaController> _logger;
        private readonly IPlazaService _plazaService;


        public PlazaController(ILogger<PlazaController> logger, IPlazaService plazaService)
        {
            _logger = logger;
            _plazaService = plazaService;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPlaza(int pageIndex, int pageSize, int? status, string keyword = null!, string plazaName = null!,
    string plazaAddress = null!)
        {
            try
            {
                // 组合查询条件
                Expression<Func<PlazaEntity, bool>> predicate = p =>
                    (string.IsNullOrWhiteSpace(keyword) ||
                     p.Name.Contains(keyword) ||
                     p.Address.Contains(keyword)) &&
                    (string.IsNullOrWhiteSpace(plazaName) || p.Name.Contains(plazaName)) &&
                    (string.IsNullOrWhiteSpace(plazaAddress) || p.Address.Contains(plazaAddress)) &&
                    (!status.HasValue || p.IsDeleted == (status.Value == 1 ? true : false));

                var Query = await _plazaService.GetPagedListByAsync(pageIndex, pageSize, predicate);
                var Total = await _plazaService.CountByAsync(predicate); // 确保统计过滤后的总数


                return Json(new
                {
                    rows = Query,
                    total = Total
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取广场数据时出错");
                return StatusCode(500, "服务器内部错误");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PlazaEntity plaza)
        {
            try
            {
                if (plaza == null)
                {
                    return BadRequest("广场数据不能为空");
                }
                plaza.UpdateTime = DateTime.Now;
                var result = await _plazaService.UpdateAsync(plaza);


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
                _logger.LogError(ex, "更新广场数据时出错");
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add(PlazaEntity plaza)
        {
            try
            {
                var result = await _plazaService.CreateAsync(plaza);

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

                _logger.LogError(ex, "添加广场数据时出错");
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, PlazaEntity plaza)
        {
            var result = await _plazaService.DeleteAsync(plaza);
            if (result)
            {
                return Json(new { success = true, message = "删除成功" });
            }
            else
            {
                return Json(new { success = false, message = "删除失败" });
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

                var result = await _plazaService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除广场数据时出错");
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> SelectBy(string name, string address)
        {
            try
            {
                // 假设 Plaza 是一个实体类，并且 PlazaService 提供了 GetOneAsync 方法
                var plaza = await _plazaService.GetManyByAsync(p => p.Name == name || p.Address == address);

                if (plaza == null)
                {
                    return Json(new { success = false, message = "未找到匹配的记录" });
                }

                return Json(new { success = true, message = "查询到", data = plaza });
            }
            catch (Exception ex)
            {
                // 记录异常日志
                return Json(new { success = false, message = "查询过程中发生错误: " + ex.Message });
            }
        }
    }
}
