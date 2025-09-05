
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Auth;
using Plaza.Net.Core.AutoFac;
using Plaza.Net.EmailService;
using Plaza.Net.Mapping;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;

using Plaza.Net.Utility.Helper;
using System.Runtime.CompilerServices;




namespace Plaza.Net.MVCAdmin
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // 配置数据库上下文
            #region SQLServer
            //builder.Services.AddDbContext<EFDbContext>(options =>
            //   options.UseSqlServer
            //   (builder
            //   .Configuration
            //   .GetConnectionString("EFDbContextConnection")!))
            //   .ConfigureIdentity();
            #endregion


            #region MySQL
            // 配置数据库上下文（使用MySQL）
            builder.Services.AddDbContext<EFDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("EFDbContextConnection")!,
                    new MySqlServerVersion(new Version(8, 0, 39)) // 根据你的MySQL版本调整
                ))
                .ConfigureIdentity();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(
                opt => opt.TokenLifespan = TimeSpan.FromHours(2));
            #endregion
            builder.Services.Configure<DataProtectionTokenProviderOptions>
                (opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            builder.Services.Configure<FromEmailConfig>(builder.Configuration
                .GetSection("FromEmailConfig"));
            // 注册 EmailSender 服务
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddHttpContextAccessor();

            //内存缓存
            //builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); // 设置Session过期时间
                options.Cookie.HttpOnly = true; // 设置为HttpOnly以提高安全性
                options.Cookie.IsEssential = true; // 标记为必要Cookie以符合GDPR
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowDev", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",   // ← 前端 uni-app H5
                            "http://localhost:5124",   // ← 如还需要
                            "http://localhost:5137"    // ← 如还需要
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            #region AutoFac注册============================================================================
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                //获取所有控制器类型并使用属性注入
                var controllerBaseType = typeof(ControllerBase);
                containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                    .PropertiesAutowired();

                containerBuilder.RegisterModule(new AutofacModuleRegister());
            });


            #endregion

            //AutoMap
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            var app = builder.Build();

            // 确保数据库迁移在应用启动时应用
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<EFDbContext>();
            //    dbContext.Database.Migrate(); // 自动应用迁移
            //}
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Authorize/Error");
            }
            app.UseCors("AllowDev");
            app.UseStaticFiles();

            app.UseRouting();

            //认证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
            app.UseSession(); // 启用Session中间件
            //启用服务器端响应缓存
            //app.UseResponseCaching();
            // 配置自定义404处理
            app.UseStatusCodePagesWithReExecute("/Authorize/Error", "?statusCode={0}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Authorize}/{action=Login}/{id?}");
  
            app.Run();
        }
    }
  
}
