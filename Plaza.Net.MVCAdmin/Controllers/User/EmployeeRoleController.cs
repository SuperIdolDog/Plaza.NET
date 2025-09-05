using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Store;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.ViewModels;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Store
{
    public class EmployeeRoleController : Controller
    {
        private readonly IEmployeeRoleService _employeeRoleService;
        private readonly IStoreService _storeService;
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        public EmployeeRoleController(
            IEmployeeRoleService employeeRoleService,
            IStoreService storeService,
            IPlazaService plazaService,
            IFloorService floorService)
        {
            _employeeRoleService = employeeRoleService;
            _storeService = storeService;
            _plazaService = plazaService;
            _floorService = floorService;
        }
        [HttpGet]

        public async Task<IActionResult> GetStoresByFloorId(int floorId)
        {
            try
            {
                // 使用服务层的方法获取指定楼层的所有店铺
                var stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId);
                return Json(stores);
            }
            catch (Exception ex)
            {
                // 返回错误信息
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId,int? floorId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<StoreEntity> stores = new List<StoreEntity>();

            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
                if(floorId.GetValueOrDefault() != 0)
                {
                    stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId!.Value);
                }    
            }

            var viewModel = new EmployeeRoleIndexViewModel
            {
                Plazas = plazas,
                Floors = floors,
                Stores = stores
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeRoleData(
     int pageIndex = 1,
     int pageSize = 10,
     int? plazaId = null,
     int? storeId = null,
     int? floorId = null,
     int? status = null,
     string name = null,
     string keyword = null)
        {
            try
            {
                // 构建查询条件
                Expression<Func<EmployeeRoleEntity, bool>> predicate = r =>
                    (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(keyword) ||
                     !string.IsNullOrWhiteSpace(name) && r.Name.Contains(name) ||
                     !string.IsNullOrWhiteSpace(keyword) && (r.Name.Contains(keyword) ||
                                                             r.Description.Contains(keyword) ||
                                                             (r.Store != null && r.Store.Name.Contains(keyword)) ||
                                                             (r.Store != null && r.Store.Floor != null && r.Store.Floor.Name.Contains(keyword)) ||
                                                             (r.Store != null && r.Store.Floor != null && r.Store.Floor.Plaza != null && r.Store.Floor.Plaza.Name.Contains(keyword)))) &&
                    (!plazaId.HasValue || (r.Store != null && r.Store.Floor != null && r.Store.Floor.PlazaId == plazaId.Value)) &&
                    (!floorId.HasValue || (r.Store != null && r.Store.Floor != null && r.Store.FloorId == floorId.Value)) &&
                    (!storeId.HasValue || (r.Store != null && r.StoreId == storeId.Value)) &&
                    (!status.HasValue || r.IsDeleted == (status.Value==1));

                // 执行分页查询
                var query = await _employeeRoleService.GetPagedListByAsync(
                    pageIndex,
                    pageSize,
                    predicate,
                    include: r => r.Include(r => r.Store)
                                  .ThenInclude(s => s.Floor)
                                  .ThenInclude(f => f.Plaza));

                // 获取总数
                var total = await _employeeRoleService.CountByAsync(predicate);

                // 构建返回结果
                var result = query.Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.StoreId,
                    StoreName = r.Store?.Name,
                    r.Store!.FloorId,
                    FloorName = r.Store?.Floor?.Name,
                    r.Store!.Floor.PlazaId,
                    PlazaName = r.Store?.Floor?.Plaza?.Name,
                    r.IsDeleted,
                    r.CreateTime,
                    r.UpdateTime,
                });

                return Json(new
                {
                    rows = result,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeRoleEntity role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest("角色数据不能为空");
                }

                role.UpdateTime = DateTime.Now;
                var result = await _employeeRoleService.UpdateAsync(role);

                return result ?
                    Json(new { success = true, message = "更新成功" }) :
                    Json(new { success = false, message = "更新失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeeRoleEntity role)
        {
            try
            {
                var result = await _employeeRoleService.CreateAsync(role);
                return result ?
                    Json(new { success = true, message = "添加成功" }) :
                    Json(new { success = false, message = "添加失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, EmployeeRoleEntity role)
        {
            var result = await _employeeRoleService.DeleteAsync(role);
            return result ?
                Json(new { success = true, message = "删除成功" }) :
                Json(new { success = false, message = "删除失败" });
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

                var result = await _employeeRoleService.DeleteRangeAsync(ids);
                return result ?
                    Json(new { success = true, message = "删除成功" }) :
                    Json(new { success = false, message = "删除失败" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }


    }


}
