using Microsoft.AspNetCore.Mvc;

using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.IServices.Sys;

using Plaza.Net.Model.ViewModels.DTO;

using AutoMapper;
using Plaza.Net.IRepository.Sys;


namespace Plaza.Net.MVCAdmin.Controllers.Sys
{
    public class SysMenuController : Controller
    {
        private readonly ISysMenuService _sysMenuService;
        public readonly ILogger<SysMenuEntity> _logger;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public SysMenuController(IMapper mapper,
            ILogger<SysMenuEntity> logger,
            ISysMenuService sysMenuService,
            IPermissionRepository permissionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _sysMenuService = sysMenuService;
            _permissionRepository = permissionRepository;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {

            var menus = await _sysMenuService.GetAllAsync();
            var dtos = _mapper.Map<List<SysMenuDTO>>(menus);
            return Json(new
            {
                rows = dtos
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SysMenuEntity menu)
        {
            try
            {
                if (menu == null)
                {
                    return BadRequest("广场数据不能为空");
                }
                menu.UpdateTime = DateTime.Now;
                var result = await _sysMenuService.UpdateAsync(menu);


                if (result)
                {
                    return Json(new { success = true, message = "更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "更新失败" });
                }
            }
            catch (Exception)
            {

                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add(SysMenuEntity menu)
        {
            try
            {
                var result = await _sysMenuService.CreateAsync(menu);

                if (result)
                {
                    return Json(new { success = true, message = "添加成功" });
                }
                else
                {
                    return Json(new { success = false, message = "添加失败" });
                }
            }
            catch (Exception)
            {


                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("未提供要删除的ID");
                }
                // 先查询数据库获取完整实体
                var existingMenu = await _sysMenuService.GetOneByIdAsync(id);
                if (existingMenu == null)
                {
                    return Json(new { success = false, message = "记录不存在" });
                }
                Console.WriteLine(existingMenu);

                var result = await _sysMenuService.DeleteAsync(existingMenu);
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


        [HttpPost]
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _sysMenuService.DeleteRangeAsync(ids);

                return Json(new
                {
                    success = result,
                    message = result ? "删除成功" : "删除失败",
                    count = result ? ids.Length : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量删除菜单失败");
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

    }
}
