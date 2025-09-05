using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Store;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.ViewModels;

using System.Linq.Expressions;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Plaza.Net.MVCAdmin.Controllers.Store
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRoleService _employeeRoleService;
        private readonly IStoreService _storeService;
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        private readonly IEmployeeService _employeeService;


        public EmployeeController(
            IEmployeeRoleService employeeRoleService,
            IStoreService storeService,
            IPlazaService plazaService,
            IFloorService floorService,
            IEmployeeService employeeService)
        {
            _employeeRoleService = employeeRoleService;
            _storeService = storeService;
            _plazaService = plazaService;
            _floorService = floorService;
            _employeeService = employeeService;
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
        public async Task<IActionResult> GetRolesByStoreId(int storeId)
        {
            try
            {
                // 使用服务层的方法获取指定楼层的所有店铺
                var employeeRoles= await _employeeRoleService.GetManyByAsync(s => s.StoreId == storeId);
                return Json(employeeRoles);
            }
            catch (Exception ex)
            {
                // 返回错误信息
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId, int? floorId, int? storeId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<StoreEntity> stores = new List<StoreEntity>();
            IEnumerable<EmployeeRoleEntity> employeeRoles = new List<EmployeeRoleEntity>();

            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
                if (floorId.GetValueOrDefault() != 0)
                {
                    stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId!.Value);
                    if (storeId.GetValueOrDefault() != 0)
                    {
                        employeeRoles=await _employeeRoleService.GetManyByAsync(s => s.StoreId== storeId!.Value);
                    }
                }
            }

            var viewModel = new EmployeeIndexViewModel
            {
                Plazas = plazas,
                Floors = floors,
                Stores = stores,
                EmployeeRoles=employeeRoles
                
            };

            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeData(
            int pageIndex,
            int pageSize,
            int? storeId,
            int? roleId,
            string name,
            string keyword)
        {
            Expression<Func<EmployeeEntity, bool>> predicate = e =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 e.Name.Contains(keyword) ||
                 e.Contact.Contains(keyword) ||
                 (e.EmployeeRole != null && e.EmployeeRole.Name.Contains(keyword)) ||
                 (e.Store != null && e.Store.Name.Contains(keyword))) &&
            (string.IsNullOrWhiteSpace(name) || (e.Name != null && e.Name.Contains(name))) &&
                (!storeId.HasValue || e.StoreId == storeId.Value) &&
                (!roleId.HasValue || e.EmployeeRoleId == roleId.Value);

            var query = await _employeeService.GetPagedListByAsync(pageIndex, pageSize, predicate,
                include: e => e.Include(e => e.EmployeeRole)
                              .Include(e => e.Store)
                              .ThenInclude(e=>e.Floor)
                              .ThenInclude(e=>e.Plaza));

            var total = await _employeeService.CountByAsync(predicate);

            var result = query.Select(e => new
            {
                e.Id,
                e.Name,
                e.Gender,
                e.Wage,
                e.CommissionRate,
                e.Contact,
                e.HireDate,
                e.Store.Floor.PlazaId,
                plazaName= e.Store.Floor.Plaza.Name,
                e.Store.FloorId,
                floorName=e.Store.Floor.Name,
                e.EmployeeRoleId,
                roleName = e.EmployeeRole?.Name,
                e.StoreId,
                storeName = e.Store?.Name,
                e.IsDeleted,
                e.CreateTime,
                e.UpdateTime,
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeEntity employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest("员工数据不能为空");
                }

                employee.UpdateTime = DateTime.Now;
                var result = await _employeeService.UpdateAsync(employee);

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
        public async Task<IActionResult> Add(EmployeeEntity employee)
        {
            try
            {
                var result = await _employeeService.CreateAsync(employee);
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
        public async Task<IActionResult> Delete(int id, EmployeeEntity employee)
        {
            var result = await _employeeService.DeleteAsync(employee);
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

                var result = await _employeeService.DeleteRangeAsync(ids);
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
