

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.ViewModels;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Device
{
    public class DeviceDataController : Controller
    {
        private readonly IDeviceDataService _deviceDataService;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly EFDbContext _dbContext;

        public DeviceDataController(
            IDeviceDataService deviceDataService,
            IDeviceService deviceService,
            IDeviceTypeService deviceTypeService,
            EFDbContext dbContext)
        {
            _deviceDataService = deviceDataService;
            _deviceService = deviceService;
            _deviceTypeService = deviceTypeService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? deviceTypeId)
        {
            // 获取所有设备类型
            var deviceTypes = await _deviceTypeService.GetAllAsync();

            // 获取所有设备
            var devices = await _deviceService.GetAllAsync();

            // 获取设备数据单位字典项
            var units = await _dbContext.Dictionary
                .Where(d => d.Name == "设备采集单位")
                .Include(di => di.DictionaryItems)
                .SelectMany(di => di.DictionaryItems)
                .ToListAsync();

            var viewModel = new DeviceDataIndexViewModel
            {
                DeviceTypes = deviceTypes,
                Devices = devices,
                DeviceDataUnits = units
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetDevicesByType(int deviceTypeId)
        {
            var devices = await _deviceService.GetManyByAsync(d => d.DeviceTypeId == deviceTypeId);
            return Json(devices);
        }

        [HttpGet]
        public async Task<IActionResult> GetData(
            int pageIndex,
            int pageSize,
            int? deviceTypeId,
            int? deviceId,
            int? unitId,
            double? minValue,
            double? maxValue,
            DateTime? startTime,
            DateTime? endTime,
            string keyword = null!)
        {
            // 组合查询条件
            Expression<Func<DeviceDataEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Device.Name.Contains(keyword) ||
                 p.DeviceDataUnitItem.Label.Contains(keyword) ||
                 p.Value.ToString().Contains(keyword)) &&
                (!deviceTypeId.HasValue || p.Device.DeviceTypeId == deviceTypeId.Value) &&
                (!deviceId.HasValue || p.DeviceId == deviceId.Value) &&
                (!unitId.HasValue || p.DeviceDataUnitItemId == unitId.Value) &&
                (!minValue.HasValue || p.Value >= minValue.Value) &&
                (!maxValue.HasValue || p.Value <= maxValue.Value) &&
                (!startTime.HasValue || p.CreateTime >= startTime.Value) &&
                (!endTime.HasValue || p.CreateTime <= endTime.Value);

            var query = await _deviceDataService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(d=>d.Device)
                               .ThenInclude(d=>d.DeviceType)
                              .Include(d => d.DeviceDataUnitItem));

            var total = await _deviceDataService.CountByAsync(predicate);

            // 转换结果
            var result = query.Select(d => new
            {
                d.Id,
                d.Value,
                d.CreateTime,
                d.Device.DeviceTypeId,
                DeviceName = d.Device.Name,
                DeviceTypeName = d.Device.DeviceType?.Name,
                UnitLabel = d.DeviceDataUnitItem.Label,
                d.DeviceId,
                d.DeviceDataUnitItemId
            });

            return Json(new
            {
                rows = result,
                Total = total,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(DeviceDataEntity deviceData)
        {
            try
            {
                if (deviceData == null)
                {
                    return BadRequest("设备数据不能为空");
                }
                var result = await _deviceDataService.CreateAsync(deviceData);

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
        public async Task<IActionResult> Edit(DeviceDataEntity deviceData)
        {
            try
            {
                if (deviceData == null)
                {
                    return BadRequest("设备数据不能为空");
                }
                deviceData.UpdateTime = DateTime.Now;
                var result = await _deviceDataService.UpdateAsync(deviceData);

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

                var result = await _deviceDataService.DeleteRangeAsync(ids);

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
