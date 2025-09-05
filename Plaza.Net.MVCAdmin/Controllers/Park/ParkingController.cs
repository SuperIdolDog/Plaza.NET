using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Plaza.Net.IServices.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels;
using Plaza.Net.Model.ViewModels.DTO;

using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Park
{
    public class ParkingController : Controller
    {
        private readonly IParkingAreaService _parkingAreaService;
        private readonly IParkingSpotService _parkingSpotService;
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        private readonly EFDbContext _dbContext;
        public ParkingController(
            IParkingAreaService parkingAreaService,
           IParkingSpotService parkingSpotService,
            IPlazaService plazaService,
        IFloorService floorService,
        EFDbContext dbContext)
        {
            _parkingAreaService = parkingAreaService;
            _parkingSpotService = parkingSpotService;
            _plazaService = plazaService;
            _floorService = floorService;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetFloorsByPlazaId(int plazaId)
        {
            var floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId);
            return Json(floors);
        }
        [HttpGet]
        public async Task<IActionResult> GetParkingAreas(int plazaId, int floorId)
        {
            var parkingAreas = await _parkingAreaService.GetManyByAsync(
                a => a.Floor.PlazaId == plazaId && a.FloorId == floorId);
            return Json(parkingAreas);
        }
        [HttpGet]
        public async Task<IActionResult> ParkingArea(int? plazaId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();

            if (plazaId.HasValue && plazaId.Value != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId.Value);
            }

            var viewModel = new ParkingAreaIndexViewModel
            {
                Parkingareas = await _dbContext.Dictionary
                    .Where(d => d.Name == "停车场区域")
                    .Include(di => di.DictionaryItems)
                    .SelectMany(di => di.DictionaryItems)
                    .ToListAsync(),
                Plazas = plazas,
                Floors = floors
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditArea(ParkingAreaEntity parkingArea)
        {
            try
            {
                if (parkingArea== null)
                {
                    return BadRequest("停车区域数据不能为空");
                }
                parkingArea.UpdateTime = DateTime.Now;
                var result = await _parkingAreaService.UpdateAsync(parkingArea);


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
        public async Task<IActionResult> AddArea(ParkingAreaEntity parkingArea)
        {
            try
            {
                var result = await _parkingAreaService.CreateAsync(parkingArea);

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
        public async Task<IActionResult> DeleteArea(int id, ParkingAreaEntity parkingArea)
        {
            var result = await _parkingAreaService.DeleteAsync(parkingArea);
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
        public async Task<IActionResult> DeleteRangeArea(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _parkingAreaService.DeleteRangeAsync(ids);

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
        public async Task<IActionResult> GetAreaData(
            int pageIndex,
            int pageSize,
            int? status,
            int? parkingAreaItemId,
            int? plazaId,
            int? floorId,
            string keyword ,
            string areaName 
        )
        {
            // 组合查询条件
            Expression<Func<ParkingAreaEntity, bool>> predicate = p =>
                // 关键字搜索（区域名称、楼层名称、编码、广场名称）
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 (p.Floor != null && p.Floor.Name.Contains(keyword)) ||
                 (p.Code != null && p.Code.Contains(keyword)) ||
                 (p.Floor != null && p.Floor.Plaza != null && p.Floor.Plaza.Name.Contains(keyword))) &&
                (string.IsNullOrWhiteSpace(areaName) || (p.Name != null && p.Name.Contains(areaName))) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1)) &&
                (!parkingAreaItemId.HasValue || p.ParkingAreaItemId == parkingAreaItemId.Value) &&
                (!plazaId.HasValue || (p.Floor != null && p.Floor.PlazaId == plazaId.Value)) &&
                (!floorId.HasValue || (p.Floor != null && p.FloorId == floorId.Value));

            // 获取分页数据
            var query = await _parkingAreaService.GetPagedListByAsync(pageIndex, pageSize, predicate,include: p => p.Include(f => f.Floor).Include(f=>f.Floor.Plaza).Include(f => f.ParkingAreaItem));

            // 获取总数
            var total = await _parkingAreaService.CountByAsync(predicate);

            // 映射结果
            var result = query.Select(p => new
            {
                p.Id,
                p.Name,
                floorName = p.Floor?.Name,
                plazaName = p.Floor?.Plaza.Name,
                parkingAreaItemLabel = p.ParkingAreaItem?.Label,
                p.ParkingAreaItemId,
                p.FloorId,
                plazaId=p.Floor?.PlazaId,
                p.IsDeleted,
                p.CreateTime,
                p.UpdateTime,
                p.Code
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }



        [HttpGet]
        public async Task<IActionResult> ParkingSpot(int? plazaId,int? floorId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<ParkingAreaEntity> parkingAreas = new List<ParkingAreaEntity>();
            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId.Value);

                // 如果有楼层ID，获取对应停车区域
                if (floorId.GetValueOrDefault() != 0)
                {
                    parkingAreas = await _parkingAreaService.GetManyByAsync(a => a.FloorId == floorId.Value);
                }
            }
            var viewModel = new ParkingSpotIndexViewModel
            {
                ParkingSpots= await _dbContext.Dictionary
                    .Where(d => d.Name == "停车位状态")
                    .Include(di => di.DictionaryItems)
                    .SelectMany(di => di.DictionaryItems)
                    .ToListAsync(),
                ParkingAreas = parkingAreas,
                Plazas = plazas,
                Floors = floors
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSpotData(
             int pageIndex,
             int pageSize,
             int? status,
             int? parkingAreaId=null!,
             int? ParkingSpotItemId = null!,
             string areaName = null,
             string keyword = null,
             string spotName = null
             
             
         )
        {
            // 组合查询条件
            Expression<Func<ParkingSpotEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 p.Code.Contains(keyword) ||
                 (p.ParkingArea != null && p.ParkingArea.Name.Contains(keyword)) ||
                 (p.ParkingArea != null && p.ParkingArea.Floor != null && p.ParkingArea.Floor.Name.Contains(keyword)) ||
                 (p.ParkingArea != null && p.ParkingArea.Floor != null && p.ParkingArea.Floor.Plaza != null && p.ParkingArea.Floor.Plaza.Name.Contains(keyword))) &&
                (string.IsNullOrWhiteSpace(spotName) || (p.Name != null && p.Name.Contains(spotName))) &&
                (string.IsNullOrWhiteSpace(areaName) || (p.ParkingArea != null && p.ParkingArea.Name.Contains(areaName))) &&
                (!ParkingSpotItemId.HasValue||(p.ParkingSpotItem!=null&&p.ParkingSpotItemId== ParkingSpotItemId.Value))&&
                (!status.HasValue || p.IsDeleted == (status.Value == 1)) &&
                (!parkingAreaId.HasValue || (p.ParkingArea != null && p.ParkingAreaId == parkingAreaId.Value));

            // 获取分页数据（包含关联实体）
            var query = await _parkingSpotService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(s=>s.ParkingSpotItem)
                                .Include(s => s.ParkingArea)
                                 .ThenInclude(a => a.Floor)
                              .ThenInclude(f => f.Plaza)
                                
                             
            );

            // 获取总数
            var total = await _parkingSpotService.CountByAsync(predicate);

            // 映射结果
            var result = query.Select(p => new
            {
                p.Id,
                p.Name,
                p.Code,
                p.ParkingAreaId,
                areaName = p.ParkingArea?.Name,
                floorId = p.ParkingArea?.FloorId,
                floorName = p.ParkingArea?.Floor?.Name,
                plazaId = p.ParkingArea?.Floor.PlazaId,
                plazaName = p.ParkingArea?.Floor?.Plaza?.Name,
                p.ParkingSpotItemId,
                parkingSpotStatus = p.ParkingSpotItem?.Label,
                p.IsDeleted,
                p.CreateTime,
                p.UpdateTime,
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditSpot(ParkingSpotEntity parkingSpot)
        {
            try
            {
                if (parkingSpot == null)
                {
                    return BadRequest("停车位数据不能为空");
                }
                parkingSpot.UpdateTime = DateTime.Now;
                var result = await _parkingSpotService.UpdateAsync(parkingSpot);


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
        public async Task<IActionResult> AddSpot(ParkingSpotEntity parkingSpot)
        {
            try
            {
                var result = await _parkingSpotService.CreateAsync(parkingSpot);

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
        public async Task<IActionResult> DeleteSpot(int id, ParkingSpotEntity parkingSpot)
        {
            var result = await _parkingSpotService.DeleteAsync(parkingSpot);
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
        public async Task<IActionResult> DeleteRangeSpot(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _parkingSpotService.DeleteRangeAsync(ids);

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
