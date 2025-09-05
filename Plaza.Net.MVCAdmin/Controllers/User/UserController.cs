using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Utility.Helper;
using System.Data;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.User
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IImageUploadService _imageUploadService;
        private readonly UserManager<UserEntity> _userManager;


        public UserController(
            IUserService userService,
            IUserRoleService userRoleService,
            IImageUploadService imageUploadService,
            UserManager<UserEntity> userManager)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _imageUploadService = imageUploadService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userRole =await _userRoleService.GetAllAsync();
            
            var model = new UserIndexViewModel { Roles = userRole };
            return View(userRole);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, string uploadType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "请选择有效的图片文件" });

                var imagePath = await _imageUploadService.UploadImageAsync(file, uploadType);

                // 返回相对路径
                return Json(new { success = true, imageUrl = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadImageFeedback(IFormFile file, [FromQuery] string uploadType = "feedback")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "请选择有效的图片文件" });

                // 1. 得到相对路径
                var relativePath = await _imageUploadService.UploadImageAsync(file, uploadType);

                // 2. 拼成完整可访问 URL
                var fullUrl = ImagePathHelper.ConvertToFullUrl(relativePath);

                // 3. 返回给前端
                return Json(new { success = true, imageUrl = fullUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadImageReview(IFormFile file, [FromQuery] string uploadType = "review")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "请选择有效的图片文件" });

                // 1. 得到相对路径
                var relativePath = await _imageUploadService.UploadImageAsync(file, uploadType);

                // 2. 拼成完整可访问 URL
                var fullUrl = ImagePathHelper.ConvertToFullUrl(relativePath);

                // 3. 返回给前端
                return Json(new { success = true, imageUrl = fullUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return Json(new { success = false, message = "图片路径不能为空" });

                // 调用 DeleteImageAsync 方法删除图片
                var isDeleted = _imageUploadService.DeleteImageAsync(imageUrl);

                if (isDeleted)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "删除图片失败" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            int pageIndex,
            int pageSize,
            string userName,
            string fullName,
            string email,
            string phone,
            string keyword = null,
            int? roleId = null,
            bool? isActive = null)
        {
            try
            {
                // 组合查询条件
                Expression<Func<UserEntity, bool>> predicate = user =>
                    (string.IsNullOrWhiteSpace(keyword) ||
                     user.UserName!.Contains(keyword) ||
                     user.Email!.Contains(keyword) ||
                     user.FullName!.Contains(keyword) ||
                     user.IDNumber!.Contains(keyword)) &&
                      (string.IsNullOrWhiteSpace(userName) || user.UserName.Contains(userName)) &&
                    (string.IsNullOrWhiteSpace(email) || user.Email.Contains(email)) &&
                     (string.IsNullOrWhiteSpace(fullName) || user.FullName!.Contains(fullName)) &&
                    (string.IsNullOrWhiteSpace(phone) || user.PhoneNumber.Contains(phone)) &&
                    (!roleId.HasValue || user.UserRoleId == roleId) &&
                    (!isActive.HasValue || user.IsDeleted == !isActive.Value);

                var query = await _userService.GetPagedListByAsync(pageIndex, pageSize, predicate,
                     include: p => p.Include(s => s.UserRole));
                var total = await _userService.CountByAsync(predicate);

                var result = query.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FullName,
                    u.IDNumber,
                    u.PhoneNumber,
                    u.AvatarUrl,
                    u.RegisterDate,
                    u.LastLoginDate,
                    u.IsDeleted,
                    u.isOnline,
                    roleName = u.UserRole?.Name,
                    u.UserRoleId
                });

                return Json(new { rows = result, Total = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEntity user)
        {
            try
            {
                if (user == null || user.Id == 0)
                {
                    return BadRequest("无效的用户数据");
                }

                // 获取现有用户
                var existingUser = await _userService.GetOneByIdAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound("未找到要编辑的用户");
                }

                // 更新可编辑字段
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;
                existingUser.IDNumber = user.IDNumber;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.UserRoleId = user.UserRoleId;
                existingUser.IsDeleted = user.IsDeleted;
                existingUser.AvatarUrl = user.AvatarUrl;

                var result = await _userService.UpdateAsync(existingUser);

                return Json(new { success = result, message = result ? "更新成功" : "更新失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdminOpUserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return BadRequest("用户数据不能为空");
                }

                // 创建用户并设置密码
                var user = new UserEntity
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                    FullName = userDTO.FullName,
                    IDNumber = userDTO.IDNumber,
                    PhoneNumber = userDTO.PhoneNumber,
                    UserRoleId = userDTO.UserRoleId,
                    AvatarUrl = userDTO.AvatarUrl,
                    IsDeleted = false,
                    RegisterDate = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user, userDTO.Password);

                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Errors.Select(e => e.Description));
                }

                return Json(new { success = true, message = "添加成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("无效的删除请求");
                }

                var user = await _userService.GetOneByIdAsync(id);
                if (user == null)
                {
                    return NotFound("未找到要删除的用户");
                }

                var result = await _userService.DeleteAsync(user);

                return Json(new { success = result, message = result ? "删除成功" : "删除失败" });
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

                var result = await _userService.DeleteRangeAsync(ids);

                return Json(new { success = result, message = result ? "删除成功" : "删除失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

       
    }
}
