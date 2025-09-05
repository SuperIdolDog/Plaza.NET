using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Plaza.Net.Auth;
using Plaza.Net.EmailService;
using Plaza.Net.IServices.Sys;
using Plaza.Net.IServices.User;
using Plaza.Net.Mapping;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Utility.Helper;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;



namespace Plaza.Net.MVCAdmin.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<UserEntity> _userManager;
        public readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<AuthorizeController> _logger;
        private readonly RoleManager<UserRoleEntity> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILoginLogService _loginLogService;
        public AuthorizeController(IMapper mapper,
            IOptionsSnapshot<JwtSetting> settings,
           IUserService userService,
           UserManager<UserEntity> userManager,
           SignInManager<UserEntity> signInManager,
           ILogger<AuthorizeController> logger,
           RoleManager<UserRoleEntity> roleManager,
           IEmailSender emailSender,
           ILoginLogService loginLogService
          )
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _loginLogService = loginLogService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            Response.StatusCode = 404;
            return View();
        }
        public IActionResult Coding()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login( UserLoginDTO loginDTO)
        {
            #region 1. 基础校验
            if (ModelState.IsValid)
                return Json(new { success = false, message = "无效的账号或密码" });
            #endregion

            #region 2. 统一字段名（前端叫 Captcha）
            var username = loginDTO.UserName.Trim();
            var password = loginDTO.Password.Trim();
            var captcha = loginDTO.Captcha?.Trim();   // ← 统一字段
            #endregion

            #region 3. 是否需要验证码
            var user = await _userManager.FindByNameAsync(username);
            bool requiresCaptcha = user != null && user.AccessFailedCount >= 3;
            if (requiresCaptcha && string.IsNullOrEmpty(captcha))
            {
                return Json(new
                {
                    success = false,
                    message = "请输入验证码",
                    requiresCaptcha = true,
                    captchaUrl = Url.Action("VerifyCode", "Authorize")
                });
            }
            if (requiresCaptcha)
            {
                var sessionCaptcha = HttpContext.Session.GetString("CheckCode");
                if (string.IsNullOrEmpty(sessionCaptcha) ||
                    !sessionCaptcha.Equals(captcha, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        success = false,
                        message = "验证码错误",
                        requiresCaptcha = true,
                        captchaUrl = Url.Action("VerifyCode", "Authorize")
                    });
                }
            }
            #endregion

            #region 4. 真正登录
            var result = await _signInManager.PasswordSignInAsync(
                username,
                password,
                loginDTO.RemeberMe,
                lockoutOnFailure: true);          // 关键：必须 true

            if (result.IsLockedOut)
                return Json(new { success = false, message = "用户被锁定,请5分钟后再次尝试" });

            if (!result.Succeeded)
            {
                // 重新取一次，拿到最新失败计数
                user = await _userManager.FindByNameAsync(username);
                if (user != null && user.AccessFailedCount >= 3)
                {
                    return Json(new
                    {
                        success = false,
                        message = "登录失败次数过多，请输入验证码",
                        requiresCaptcha = true,
                        captchaUrl = Url.Action("VerifyCode", "Authorize")
                    });
                }

                return Json(new { success = false, message = "无效的账号或密码" });
            }
            #endregion

            #region 5. 成功登录
            // 清零失败计数
            await _userManager.ResetAccessFailedCountAsync(user!);

            // 更新最后登录时间等
            user.LastLoginDate = DateTime.Now;
            user.isOnline = 1;
            await _userService.UpdateAsync(user);

            await _loginLogService.LogSuccessfulLoginAsync(
                userId: user.Id,
                ipAddress: HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                           ?? HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString(),
                deviceInfo: HttpContext.Request.Headers["User-Agent"].ToString());

            // 写 Cookie（示例）
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim("AvatarUrl", user.AvatarUrl ?? string.Empty)
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = loginDTO.RemeberMe });

            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            #endregion
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Authorize");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return Json(new { success = false, message = "请输入有效的邮箱" });
            }

            // 检查邮箱是否已注册
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "该邮箱已被注册" });
            }

            // 生成验证码
            var verificationCode = RandomNumberHelper.Str(6, true);

            // 将验证码和过期时间保存到Session中
            HttpContext.Session.SetString("VerificationCode", verificationCode);
            HttpContext.Session.SetString("VerificationCodeExpiry", DateTime.Now.AddMinutes(5).ToString("o"));
            Console.WriteLine("验证码:" + verificationCode);
            // 发送验证码邮件
            try
            {
                var message = new Message(
                    name: "系统邮件",
                    address: email,
                   subject: "SuperAdmin",
                   content: $"您的注册验证码是：{verificationCode}，请在5分钟内使用。"
                );

                await _emailSender.SendEmailAsync(message);

                return Json(new { success = true, message = "验证码已发送，请查收邮件" });
            }
            catch (Exception ex)
            {
                // 记录日志
                _logger.LogError(ex, "发送验证码邮件失败");
                return Json(new { success = false, message = "发送验证码失败，请稍后再试" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDTO registerDTO)
        {
            // 验证验证码
            var storedCode = HttpContext.Session.GetString("VerificationCode");
            var storedCodeExpiryStr = HttpContext.Session.GetString("VerificationCodeExpiry");

            if (string.IsNullOrWhiteSpace(storedCode) ||
                string.IsNullOrWhiteSpace(storedCodeExpiryStr) ||
                !DateTime.TryParse(storedCodeExpiryStr, out var storedCodeExpiry) ||
                DateTime.Now > storedCodeExpiry ||
                storedCode != registerDTO.Code
                )
            {

                return Json(new
                {
                    success = false,
                    message = "验证码无效或已过期，请重新获取"
                });
            }

            // 清除已使用的验证码
            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("VerificationCodeExpiry");

            // 原有的注册逻辑保持不变
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "表单验证失败"
                });
            }

            // 验证密码和确认密码是否匹配
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "密码和确认密码不匹配"
                });
            }

            var user = UserMapper.MapToUser(registerDTO);
            var userWithSameUserName = await _userManager.FindByNameAsync(registerDTO.UserName);
            if (userWithSameUserName != null)
            {
                return Json(new
                {
                    success = false,
                    message = "用户名已存在"
                });
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (userWithSameEmail != null)
            {
                return Json(new
                {
                    success = false,
                    message = "邮箱已被使用！"
                });
            }

            var defaultRoleName = "Guest";
            var defaultRole = await _roleManager.FindByNameAsync("Guest");
            if (defaultRole == null)
            {
                defaultRole = new UserRoleEntity
                {
                    Name = defaultRoleName,
                    NormalizedName = defaultRoleName.ToUpperInvariant()
                };
                var roleResult = await _roleManager.CreateAsync(defaultRole);
                if (!roleResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = "角色创建失败"
                    });
                }
            }
            user.Code = storedCode;
            user.UserRoleId = defaultRole.Id;

            // 先创建用户但不确认邮箱
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                    _logger.LogWarning(error.Description);
                }
                return Json(new
                {
                    success = false,
                    message = "用户注册失败"
                });
            }

            return Json(new
            {
                success = true,
                message = "用户注册成功",
                redirectUrl = Url.Action("Login", "Authorize")
            });
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {

            return View();
        }
        /// <summary>
        /// 发送忘记密码邮箱验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "请输入有效的邮箱地址"
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new
                {
                    success = true,
                    message = "验证码已发送至您的邮箱，请输入验证码重置密码"
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            Console.WriteLine("生成的令牌: " + token);
            var resetMessage = new Message
            (
                $"{user.UserName}",
                user.Email,
                "密码重置验证码",
                $"您的密码重置验证码是：{token}，请在5分钟内使用。"
            );
            await _emailSender.SendEmailAsync(resetMessage);

            // 将验证码和过期时间保存到Session中
            HttpContext.Session.SetString("PasswordResetToken", token);
            HttpContext.Session.SetString("TokenExpiry", DateTime.Now.AddMinutes(5).ToString("o"));

            return Json(new
            {
                success = true,
                message = "验证码已发送至您的邮箱，请输入验证码重置密码"
            });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ForgotPasswordDTO resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "两次密码不一致"
                });
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "用户不存在"
                });
            }

            // 从Session中获取验证码和过期时间
            var storedToken = HttpContext.Session.GetString("PasswordResetToken");
            var storedExpiry = HttpContext.Session.GetString("TokenExpiry");
            Console.WriteLine("Session存储的验证码" + storedToken);
            Console.WriteLine("Session存储的发送验证码时间" + storedExpiry);
            if (string.IsNullOrEmpty(storedToken) || string.IsNullOrEmpty(storedExpiry))
            {
                return Json(new
                {
                    success = false,
                    message = "验证码无效或已过期"
                });
            }

            // 解析过期时间
            if (!DateTime.TryParse(storedExpiry, out var expiryTime))
            {
                return Json(new
                {
                    success = false,
                    message = "验证码无效或已过期"
                });
            }

            // 验证验证码是否匹配且未过期
            if (storedToken != resetPasswordModel.Code || expiryTime < DateTime.Now)
            {
                return Json(new
                {
                    success = false,
                    message = "验证码无效或已过期"
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            resetPasswordModel.Code = token;
            Console.WriteLine(token);
            // 重置密码
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Code, resetPasswordModel.Password);
            Console.WriteLine("重置密码信息：" + result);
            Console.WriteLine(resetPasswordModel.Code);
            Console.WriteLine(resetPasswordModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return Json(new
                {
                    success = false,
                    message = "密码重置失败"
                });
            }

            // 清除Session中的验证码
            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("VerificationCodeExpiry");

            return Json(new
            {
                success = true,
                message = "密码重置成功",
                redirectUrl = Url.Action("Login", "Authorize")
            });
        }
        public IActionResult VerifyCode()
        {
            string code = "";
            Bitmap bitmap = VerifyCodeHelper.GenerateCaptcha(out code);
            HttpContext.Session.SetString("CheckCode", code);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }

    }
}
