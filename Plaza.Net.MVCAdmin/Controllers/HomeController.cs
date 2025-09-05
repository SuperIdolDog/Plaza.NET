using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Order;
using Plaza.Net.IServices.Sys;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels;



namespace Plaza.Net.MVCAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISysMenuService _sysmenuService;
        private readonly IPlazaService _plazaService;
        private readonly INotificationService _notificationService;
        public readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IUserService _userService;
        private readonly EFDbContext _dbContext;
        private readonly IImageUploadRepository _imageUploadRepository;
        private readonly IOrderService _orderService;

        public HomeController(
            ILogger<HomeController> logger,
            ISysMenuService sysmenuService,
            IPlazaService plazaService,
            INotificationService notificationService,
                 SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager,
            IUserService userService,
            EFDbContext dbContext,
            IImageUploadRepository imageUploadRepository,
             IOrderService orderService
        )
        {
            _logger = logger;
            _sysmenuService = sysmenuService;
            _plazaService = plazaService;
            _notificationService = notificationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _dbContext = dbContext;
            _imageUploadRepository = imageUploadRepository;
            _orderService = orderService;
        }

        [Authorize]
        public  async Task<IActionResult> Index()
        {
            var username = User.Identity!.Name;
            var user = await _dbContext.Users
                  .Where(u => u.UserName == username)
                  .FirstOrDefaultAsync();
           
            if (user!=null)
            {
                var userId = user.Id;
                var roleId=user.UserRoleId;

                var menuItems = await _sysmenuService.GetAllMenusByRoleIdAsync(roleId);

                var notifiacations = await _dbContext
                    .Notification
                    .Where(n => n.UserId == userId)
                    .Where(n => n.IsRead == false)
                    .ToListAsync();
                var avatar = string.IsNullOrEmpty(user.AvatarUrl)
             ? null
             : $"/{user.AvatarUrl.TrimStart('/')}";
                var notNum= notifiacations.Count();
                var viewModel = new IndexViewModel
                {
                    MenuItems = menuItems,
                    Notifications = notifiacations,
                    NotificationCount = notNum,
                    Avatar= avatar
                };
                return View(viewModel);
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            var username = User.Identity!.Name;
            var user = await _dbContext.Users
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var viewModel = new UserInfoViewModel
                {
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    IDNumber = user.IDNumber,
                    AvatarUrl = user.AvatarUrl,
                    PhoneNumber = user.PhoneNumber
                };
                return View(viewModel);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = errors.FirstOrDefault() ?? "表单验证失败" });
            }

            var username = User.Identity!.Name;
            var user = await _dbContext.Users
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Json(new { success = false, message = "用户不存在" });
            }

            try
            {
                // 检查邮箱是否被更改
                if (user.Email != model.Email)
                {
                    // 检查邮箱是否被其他用户使用
                    var userWithNewEmailExists = await _dbContext.Users
                        .AnyAsync(u => u.Email == model.Email && u.Id != user.Id);

                    if (userWithNewEmailExists)
                    {
                        return Json(new { success = false, message = "邮箱已被使用" });
                    }
                    user.Email = model.Email;
                }
                // 更新用户信息
                user.FullName = model.FullName;
 
                user.IDNumber = model.IDNumber;
                user.PhoneNumber = model.PhoneNumber;

                // 检查用户名是否被更改
                if (user.UserName != model.UserName)
                {
                    // 检查新用户名是否已被使用
                    var userWithNewUsernameExists = await _dbContext.Users
                        .AnyAsync(u => u.UserName == model.UserName);

                    if (userWithNewUsernameExists)
                    {
                        return Json(new { success = false, message = "用户名已被使用" });
                    }
                  
                    // 更新用户名
                    user.UserName = model.UserName;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "更新失败" });
                }

                // 如果用户名被更新，注销用户
                if (user.UserName != username)
                {
                    await _signInManager.SignOutAsync();

                    return Json(new
                    {
                        success = true,
                        message = "用户名已更新，请重新登录",
                        user = new
                        {
                            userName = user.UserName,
                            fullName = user.FullName,
                            email = user.Email,
                            iDNumber = user.IDNumber,
                            phoneNumber = user.PhoneNumber
                        },
                        redirectUrl = Url.Action("Login", "Authorize") 
                    });
                }
               
                return Json(new
                {
                    success = true,
                    message = "个人信息更新成功",
                    user = new
                    {
                        userName = user.UserName,
                        fullName = user.FullName,
                        email = user.Email,
                        iDNumber = user.IDNumber,
                        phoneNumber = user.PhoneNumber
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"更新失败: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "请选择文件" });
            }

            try
            {
                // 使用 ImageUploadRepository 上传文件
                var uploadPath = "users"; // 上传到 images/users 目录
                var relativePath = await _imageUploadRepository.UploadImageAsync(file, uploadPath);

                var username = User.Identity!.Name;
                var user = await _dbContext.Users
                    .Where(u => u.UserName == username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Json(new { success = false, message = "用户不存在" });
                }

                // 删除旧头像（如果存在）
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    _imageUploadRepository.DeleteImageAsync(user.AvatarUrl);
                }

                // 更新用户头像路径
                user.AvatarUrl = relativePath;
                await _dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "头像上传成功",
                    avatarUrl = $"/{relativePath.TrimStart('/')}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = errors.FirstOrDefault() ?? "表单验证失败" });
            }

            var username = User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Json(new { success = false, message = "用户不存在" });
            }

            try
            {
                // 验证旧密码
                var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!isOldPasswordValid)
                {
                    return Json(new { success = false, message = "旧密码不正确" });
                }

                // 检查新密码和确认密码是否匹配
                if (model.NewPassword != model.ConfirmPassword)
                {
                    return Json(new { success = false, message = "新密码和确认密码不匹配" });
                }

                // 修改密码
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "密码修改失败" });
                }
                await _signInManager.SignOutAsync();
                return Json(new { success = true, message = "密码修改成功", redirectUrl = Url.Action("Login", "Authorize") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"密码修改失败: {ex.Message}" });
            }
        }
        public async Task<IActionResult> Privacy()
        {
            var plazacount = await _plazaService.CountAsync();
            var usercount = await _userService.CountAsync();
            var ordercount = await _orderService.CountAsync();
            var ordermoney = await _orderService.GetAllAsync();

            var model = new PrivacyViewModel
            {
                PlazaCount = plazacount,
                UserCount = usercount,
                OrderCount = ordercount,
                TotalOrderMoney = ordermoney.Sum(o => o.TotalAmount) 
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Map()
        {
            var query = await  _plazaService.GetAllAsync();
            return Json(new
            {
                data = query
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }


}
