using Microsoft.Extensions.DependencyInjection;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model;
using Microsoft.AspNetCore.Identity;
using Plaza.Net.Utility.Helper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace Plaza.Net.Auth
{
    public static class AuthorizationSetup
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            // 注入数据保护
            services.AddDataProtection();

            // 配置Identity
            services.AddIdentity<UserEntity, UserRoleEntity>(opt =>
            {
                // 密码策略
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                opt.User.RequireUniqueEmail = true;
                //opt.SignIn.RequireConfirmedEmail = true;

                // 用户设置
                opt.User.RequireUniqueEmail = true;
                //opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ \u4e00-\u9fff";
                opt.User.AllowedUserNameCharacters = null;
                // 锁定设置
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // Token提供者
                opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddUserValidator<CustomUserValidator<UserEntity>>()
            .AddEntityFrameworkStores<EFDbContext>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt =>
            {
                opt.LoginPath = "/Authorize/Login";
                opt.AccessDeniedPath = "/Home/Error";
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
            services.AddControllersWithViews();
            return services;
        }
    }
}
