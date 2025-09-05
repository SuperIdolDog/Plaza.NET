

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.ViewModels;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Basic
{
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IFloorService _floorService;
        private readonly IPlazaService _plazaService;
        private readonly EFDbContext _dbContext;

        public DeviceController(
            IDeviceService deviceService,
            IDeviceTypeService deviceTypeService,
            IFloorService floorService,
            IPlazaService plazaService,
            EFDbContext dbContext)
        {
            _deviceService = deviceService;
            _deviceTypeService = deviceTypeService;
            _floorService = floorService;
            _plazaService = plazaService;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int? plazaId)
        {
            // 获取所有广场
            var plazas = await _plazaService.GetAllAsync();

            // 获取广场对应的楼层
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            if (plazaId.HasValue && plazaId.Value != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId.Value);
            }

            var viewModel = new DeviceIndexViewModel
            {
                DeviceTypes = await _deviceTypeService.GetAllAsync(),
                Floors = floors,
                Plazas = plazas,
                DeviceStatuses = await _dbContext.Dictionary
                    .Where(d => d.Name == "设备状态")
                    .Include(di => di.DictionaryItems)
                    .SelectMany(di => di.DictionaryItems)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetFloorsByPlazaId(int plazaId)
        {
            var floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId);
            return Json(floors);
        }

        [HttpGet]
        public async Task<IActionResult> GetData(
          int pageIndex,
          int pageSize,
          int? DeviceStatusItemId,
          int? deviceTypeId,
          int? floorId,
          int? plazaId,
          string keyword = null!,
          string deviceName = null!,
          string location = null!)
        {
            // 组合查询条件
            Expression<Func<DeviceEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 p.Location.Contains(keyword) ||
                 p.DeviceType != null && p.DeviceType.Name.Contains(keyword) ||
                 p.Floor != null && p.Floor.Name.Contains(keyword) ||
                 p.DeviceStatusItem != null && p.DeviceStatusItem.Label.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(deviceName) || p.Name.Contains(deviceName)) &&
                (string.IsNullOrWhiteSpace(location) || p.Location.Contains(location)) &&
                (!DeviceStatusItemId.HasValue || p.DeviceStatusItem != null && p.DeviceStatusItemId == DeviceStatusItemId.Value) &&
                (!deviceTypeId.HasValue || p.DeviceTypeId == deviceTypeId.Value) &&
                (!plazaId.HasValue || (p.Floor != null && p.Floor.PlazaId == plazaId.Value)) &&
                (!floorId.HasValue || p.FloorId == floorId.Value);

            var query = await _deviceService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(d => d.DeviceType)
                              .Include(d => d.Floor)
                              .Include(f => f.Floor.Plaza)
                              .Include(d => d.DeviceStatusItem));

            var total = await _deviceService.CountByAsync(predicate);

            // 转换结果，确保返回设备类型名称、楼层名称和状态文本
            var result = query.Select(d => new
            {
                d.Id,
                d.Name,
                d.Location,
                d.DeviceStatusItemId,
                d.LastMaintenanceDate,
                d.IsDeleted,
                d.CreateTime,
                d.UpdateTime,
                d.DeviceTypeId,
                deviceTypeName = d.DeviceType?.Name,
                d.FloorId,
                floorName = d.Floor?.Name,
                floorPlazaId = d.Floor?.PlazaId,
                floorPlazaName = d.Floor?.Plaza?.Name,
                statusText = d.DeviceStatusItem?.Label
            });

            return Json(new
            {
                rows = result,
                Total = total,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DeviceEntity device)
        {
            try
            {
                if (device == null)
                {
                    return BadRequest("设备数据不能为空");
                }
                device.UpdateTime = DateTime.Now;
                var result = await _deviceService.UpdateAsync(device);

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
        public async Task<IActionResult> Add(DeviceEntity device)
        {
            try
            {
                var result = await _deviceService.CreateAsync(device);

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
        public async Task<IActionResult> Delete(int id, DeviceEntity device)
        {
            var result = await _deviceService.DeleteAsync(device);
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

                var result = await _deviceService.DeleteRangeAsync(ids);

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
