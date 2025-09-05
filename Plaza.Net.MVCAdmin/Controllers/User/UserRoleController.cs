using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels.DTO;
using System.Linq.Expressions;
using System.Security;

namespace Plaza.Net.MVCAdmin.Controllers.User
{
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IPermissionService _permissionService;
        private readonly ISysMenuService _sysMenuService;
        private readonly EFDbContext _dbContext;

        public UserRoleController(
            IUserRoleService userRoleService,
            IPermissionService permissionService,
            ISysMenuService sysMenuService,
            EFDbContext dbContext)
        {
            _userRoleService = userRoleService;
            _permissionService = permissionService;
            _sysMenuService = sysMenuService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles(
            int pageIndex,
            int pageSize,
            int? status,
            string name,
            string description,
            string keyword = null!,
            string code=null!
             )
        {
            try
            {
                Expression<Func<UserRoleEntity, bool>> predicate = e =>
                               (string.IsNullOrWhiteSpace(keyword) ||
                                e.Name.Contains(keyword) ||
                                e.Description!.Contains(keyword) ||
                                e.Code!.Contains(keyword))&&
                                (string.IsNullOrWhiteSpace(code) || (e.Code != null && e.Code.Contains(code))) &&
                                (string.IsNullOrWhiteSpace(name) || (e.Name != null && e.Name.Contains(name)))&&
                                (string.IsNullOrWhiteSpace(description) || (e.Description != null && e.Description.Contains(description))) &&
                                (!status.HasValue || e.IsDeleted == (status.Value == 1)) ;

                var query = await _userRoleService.GetPagedListByAsync(pageIndex, pageSize, predicate);
                var total = await _userRoleService.CountByAsync(predicate);

                return Json(new { rows = query, Total = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            try
            {
                // 获取所有菜单（显示在ztree上）
                var allMenus = await _sysMenuService.GetAllAsync();
                // 获取角色已分配的所有菜单ID(复选框上打勾)
                var assignedMenuIds = await _dbContext.MenuPermission
                    .Where(mp => mp.Permission.RoleId == roleId)
                    .Select(mp => mp.MenuId)
                    .ToListAsync();
                // 构建菜单树并添加权限信息

                var menuDtos = allMenus.Select(menu => new MenuWithPermissionDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    ParentId = menu.ParentId,
                    Order = menu.Order,
                    HasPermission = assignedMenuIds.Contains(menu.Id),
                    Children = new List<MenuWithPermissionDto>()
                }).ToList();

                var menuDict = menuDtos.ToDictionary(m => m.Id);

                foreach (var menuDto in menuDtos)
                {
                    if (menuDto.ParentId.HasValue && menuDto.ParentId != 0 &&
                        menuDict.TryGetValue(menuDto.ParentId.Value, out var parent))
                    {
                        parent.Children.Add(menuDto);
                    }
                }
                var menuTree = menuDtos
                     .Where(m => m.ParentId == null || m.ParentId == 0)
                     .OrderBy(m => m.Order)
                     .ToList();
                return Ok(new { success = true, data = menuTree });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AssignPermissions(int roleId, int[] menuIds)
        {

            Console.WriteLine(roleId);
            Console.WriteLine(menuIds);
            try
            {
                // 查询验证传入角色是否存在
                var role = await _userRoleService.GetOneByIdAsync(roleId);
             
                if (role == null)
                    return NotFound(new { success = false, message = "角色不存在" });

                // 2. 根据角色id获取当前权限
                var currentPermissions = await _dbContext.MenuPermission
                    .Include(mp => mp.Permission)
                    .Where(mp => mp.Permission.RoleId == roleId)
                    .ToListAsync();

                // 3. Process menu IDs (null-safe)
                var newMenuIds = menuIds?.Distinct().ToList() ?? new List<int>();

                // 4. Calculate differences
                var currentMenuIds = currentPermissions.Select(mp => mp.MenuId).ToList();

                //删除权限
                var menusToRemove = currentMenuIds.Except(newMenuIds).ToList();
                if (menusToRemove.Any())
                {
                    var toRemove = currentPermissions
                        .Where(mp => menusToRemove.Contains(mp.MenuId))
                        .ToList();

                    _dbContext.MenuPermission.RemoveRange(toRemove);

                    // Remove associated permissions
                    var permissionIds = toRemove.Select(mp => mp.PermissionId).ToList();
                    var permissions = await _permissionService.GetManyByAsync(
                        p => permissionIds.Contains(p.Id));
                    var permissionIdsToDelete = permissions.Select(p => p.Id).ToArray();
                    await _permissionService.DeleteRangeAsync(permissionIdsToDelete);
                }

                // Menus to add
                var menusToAdd = newMenuIds.Except(currentMenuIds).ToList();
                if (menusToAdd.Any())
                {
                    var menus = await _sysMenuService.GetManyByAsync(m => menusToAdd.Contains(m.Id));
                    var menuDict = menus.ToDictionary(m => m.Id);

                    foreach (var menuId in menusToAdd)
                    {
                        if (!menuDict.TryGetValue(menuId, out var menu))
                            continue;

                        var permission = new PermissionEntity
                        {
                            RoleId = roleId,
                            Name = $"{role.Name} - {menu.Name} 权限", // 修复：确保使用 role.Name
                            Description = $"自动生成的 {role.Name} 角色对 {menu.Name} 菜单的权限",
                        };

                        await _permissionService.CreateAsync(permission);

                        var menuPermission = new MenuPermissionEntity
                        {
                            MenuId = menuId,
                            PermissionId = permission.Id
                        };

                        await _dbContext.MenuPermission.AddAsync(menuPermission);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    success = false,
                    message = $"权限分配失败: {ex.Message}"
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleEntity role)
        {
            try
            {
                if (role == null) return BadRequest("角色数据不能为空");

                var result = await _userRoleService.UpdateAsync(role);

                return Json(new { success = result, message = result ? "更新成功" : "更新失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserRoleEntity role)
        {
            try
            {
                if (role == null) return BadRequest("角色数据不能为空");

                var result = await _userRoleService.CreateAsync(role);

                return Json(new { success = result, message = result ? "添加成功" : "添加失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any()) return BadRequest("未提供要删除的ID");

                var result = await _userRoleService.DeleteRangeAsync(ids);

                return Json(new { success = result, message = result ? "删除成功" : "删除失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
