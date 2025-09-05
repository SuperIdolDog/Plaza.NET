using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Basic
{
    public class StoreTypeController : Controller
    {
        private readonly IStoreTypeService _storeTypeService;
        private readonly EFDbContext _dbContext;

        public StoreTypeController(
            IStoreTypeService storeTypeService,
            EFDbContext dbContext)
        {
            _storeTypeService = storeTypeService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public  IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreTypeData(
            int pageIndex,
            int pageSize,
            string keyword = null,
            int? status = null)
        {
            // 组合查询条件
            Expression<Func<StoreTypeEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword)) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1));

            // 获取分页数据
            var query = await _storeTypeService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate);

            // 获取总数
            var total = await _storeTypeService.CountByAsync(predicate);

            // 映射结果
            var result = query.Select(p => new
            {
                p.Id,
                p.Name,
                p.IsDeleted,
                p.CreateTime,
                p.UpdateTime
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddStoreType(StoreTypeEntity storeType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(storeType.Name))
                {
                    return BadRequest("店铺类型名称不能为空");
                }

                var result = await _storeTypeService.CreateAsync(storeType);

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
        public async Task<IActionResult> EditStoreType(StoreTypeEntity storeType)
        {
            try
            {
                if (storeType == null || string.IsNullOrWhiteSpace(storeType.Name))
                {
                    return BadRequest("店铺类型数据不能为空");
                }

                storeType.UpdateTime = DateTime.Now;
                var result = await _storeTypeService.UpdateAsync(storeType);

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
        public async Task<IActionResult> DeleteRangeStoreType(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _storeTypeService.DeleteRangeAsync(ids);

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
