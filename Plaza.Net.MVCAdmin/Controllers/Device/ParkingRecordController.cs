using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.ViewModels;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Device
{
    public class ParkingRecordController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IParkingRecordService _parkingRecordService;

        public ParkingRecordController(
            IDeviceService deviceService,
            IParkingRecordService parkingRecordService)
        {
            _deviceService = deviceService;
            _parkingRecordService = parkingRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 获取所有设备
            var devices = await _deviceService.GetAllAsync();

            var viewModel = new ParkingRecordIndexViewModel
            {
                Devices = devices
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingRecords(
            int pageIndex,
            int pageSize,
            int? deviceId,
            string plateNumber = null!)
        {
            // 组合查询条件
            Expression<Func<ParkingRecordEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(plateNumber) ||
                 p.PlateNumber.Contains(plateNumber)) &&
                (!deviceId.HasValue || p.DeviceId == deviceId.Value);

            var query = await _parkingRecordService.GetPagedListByAsync(
                pageIndex,
                pageSize,
                predicate,
                include: p => p.Include(pr => pr.Device));

            var total = await _parkingRecordService.CountByAsync(predicate);

            // 转换结果
            var result = query.Select(pr => new
            {
                pr.Id,
                pr.PlateNumber,
                pr.EntryTime,
                pr.ExitTime,
                pr.ParkingFee,
                DeviceName = pr.Device.Name,
                pr.DeviceId
            });

            return Json(new
            {
                rows = result,
                Total = total,
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await _deviceService.GetAllAsync();
            return Json(devices);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ParkingRecordEntity parkingRecord)
        {
            try
            {
                if (parkingRecord == null)
                {
                    return BadRequest("停车记录不能为空");
                }

                parkingRecord.EntryTime = DateTime.Now; // 设置默认进入时间为当前时间
                var result = await _parkingRecordService.CreateAsync(parkingRecord);

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
        public async Task<IActionResult> Edit(ParkingRecordEntity parkingRecord)
        {
            try
            {
                if (parkingRecord == null)
                {
                    return BadRequest("停车记录不能为空");
                }

                var result = await _parkingRecordService.UpdateAsync(parkingRecord);

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

                var result = await _parkingRecordService.DeleteRangeAsync(ids);

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
