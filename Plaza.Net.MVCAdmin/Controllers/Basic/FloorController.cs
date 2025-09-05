
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels;
using System;
using System.Linq.Expressions;


namespace Plaza.Net.MVCAdmin.Controllers.Basic
{
    public class FloorController : Controller
    {
        private readonly IFloorService _floorService;
        private readonly IPlazaService _plazaService;
        private readonly EFDbContext _dbContext;

        public FloorController(
            IFloorService floorService,
            IPlazaService plazaService,
            EFDbContext dbContext)
        {
            _floorService = floorService;
            _plazaService = plazaService;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new FloorIndexViewModel
            {
             Floors = await _dbContext.Dictionary
            .Where(d => d.Name == "楼层")
            .Include(di => di.DictionaryItems)
            .SelectMany(di => di.DictionaryItems)
            .ToListAsync(),
                Plazas = await _plazaService.GetAllAsync()
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetData(
            int pageIndex,
            int pageSize,
            int? status,
            int? flooritem,
            string keyword = null!,
            string plazaName = null!,
            string floorName = null!)
        {
            // 组合查询条件
            Expression<Func<FloorEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 p.Description.Contains(keyword) ||
                 p.Code!.Contains(keyword) ||
                 p.Plaza != null && p.Plaza.Name.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(plazaName) || p.Plaza != null && p.Plaza.Name.Contains(plazaName)) &&
                (string.IsNullOrWhiteSpace(floorName) || p.Name.Contains(floorName)) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1)) &&
                (!flooritem.HasValue || p.FloorItemId == flooritem.Value);

            var query = await _floorService.GetPagedListByAsync(pageIndex, pageSize, predicate, include: p => p.Include(f => f.Plaza).Include(f=>f.FloorItem));
            var Total = await _floorService.CountByAsync(predicate);

            // 转换结果，确保返回广场名称而不是ID
            var result = query.Select(f => new
            {
                f.Id,
                f.Name,
                f.Description,
                f.Code,
                f.IsDeleted,
                f.CreateTime,
                f.UpdateTime,
                f.FloorItemId,
                flooritemName=f.FloorItem?.Label,
                f.PlazaId,
                plazaName = f.Plaza.Name,
            });

            return Json(new
            {
                rows = result,
                total = Total,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(FloorEntity floor)
        {
            try
            {
                if (floor == null)
                {
                    return BadRequest("广场数据不能为空");
                }
                floor.UpdateTime = DateTime.Now;
                var result = await _floorService.UpdateAsync(floor);


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
        public async Task<IActionResult> Add(FloorEntity floor)
        {
            try
            {
                var result = await _floorService.CreateAsync(floor);

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
        public async Task<IActionResult> Delete(int id, FloorEntity floor)
        {
            var result = await _floorService.DeleteAsync(floor);
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

                var result = await _floorService.DeleteRangeAsync(ids);

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

