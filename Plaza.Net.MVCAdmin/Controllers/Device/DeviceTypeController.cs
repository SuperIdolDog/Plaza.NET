using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model.Entities.Device;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Device
{
    public class DeviceTypeController : Controller
    {
        private readonly IDeviceTypeService _deviceTypeService;

        public DeviceTypeController(IDeviceTypeService deviceTypeService)
        {
            _deviceTypeService = deviceTypeService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetData(
               int pageIndex,
               int pageSize,
               int? status,
               string keyword = null!,
               string typeName = null!,
               string manufacturer = null!)
        {
            // 组合查询条件
            Expression<Func<DeviceTypeEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 p.Description.Contains(keyword) ||
                 p.Manufacturer.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(typeName) || p.Name.Contains(typeName)) &&
                (string.IsNullOrWhiteSpace(manufacturer) || p.Manufacturer.Contains(manufacturer)) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1));

            var query = await _deviceTypeService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate
                );

            var total = await _deviceTypeService.CountByAsync(predicate);

            // 转换结果
            var result = query.Select(dt => new
            {
                dt.Id,
                dt.Name,
                dt.Description,
                dt.Manufacturer,
                dt.IsDeleted,
                dt.CreateTime,
                dt.UpdateTime
            });

            return Json(new
            {
                rows = result,
                Total = total,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DeviceTypeEntity deviceType)
        {
            try
            {
                if (deviceType == null)
                {
                    return BadRequest("设备类型数据不能为空");
                }
                deviceType.UpdateTime = DateTime.Now;
                var result = await _deviceTypeService.UpdateAsync(deviceType);

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
        public async Task<IActionResult> Add(DeviceTypeEntity deviceType)
        {
            try
            {
                var result = await _deviceTypeService.CreateAsync(deviceType);

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
        public async Task<IActionResult> Delete(int id, DeviceTypeEntity deviceType)
        {
            var result = await _deviceTypeService.DeleteAsync(deviceType);
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

                var result = await _deviceTypeService.DeleteRangeAsync(ids);

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
