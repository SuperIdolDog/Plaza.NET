using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plaza.Net.Auth;
using Plaza.Net.Core.AutoFac;
using Plaza.Net.Mapping;
using Plaza.Net.Model;
using System.Text;
using System;
using static Plaza.Net.Utility.Helper.ImagePathHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Plaza.Net.Model.ViewModels.Pay;
using Aop.Api;



namespace Plaza.Net.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //读取配置文件中JWT的信息，然后通过Configuration配置系统注入到Controller层进行授权
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtConfig"));



                    // 配置数据库上下文
            #region SQLServer
            //builder.Services.AddDbContext<EFDbContext>(options =>
            //   options.UseLazyLoadingProxies().UseSqlServer
            //   (builder
            //   .Configuration
            //   .GetConnectionString("EFDbContextConnection")!))
            //   .ConfigureIdentity();
            #endregion


            #region MySQL
            // 配置数据库上下文（使用MySQL）
            builder.Services.AddDbContext<EFDbContext>(options =>
                options.UseLazyLoadingProxies().UseMySql(
                    builder.Configuration.GetConnectionString("EFDbContextConnection")!,
                    new MySqlServerVersion(new Version(8, 0, 39)) // 根据你的MySQL版本调整
                ))
                .ConfigureIdentity();
            #endregion


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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            #region 支付宝支付
            builder.Services.Configure<AlipayOptions>(builder.Configuration.GetSection("AliPay"));
            builder.Services.AddSingleton<IAopClient>(sp =>
            {
                var cfg = sp.GetRequiredService<IOptions<AlipayOptions>>().Value;
                Console.WriteLine($"绑定的 Charset: {cfg.Charset ?? "null"}");
                Console.WriteLine($"绑定的 SignType: {cfg.SignType ?? "null"}");
                Console.WriteLine($"绑定的 GatewayUrl: {cfg.GatewayUrl ?? "null"}");
                //return new DefaultAopClient(
                //   cfg.GatewayUrl,
                //    cfg.AppId,

                //    cfg.PrivateKey,
                //      cfg.AlipayPublicKey,
                //      cfg.Charset,
                //      cfg.SignType

                //    );
                if (string.IsNullOrWhiteSpace(cfg.Charset))
                {
                    throw new InvalidOperationException("AlipayOptions.Charset 绑定失败，值为空。请检查配置文件。");
                }
                var charset1 = cfg.Charset ?? "UTF-8";
                return new DefaultAopClient(
                    serverUrl: cfg.GatewayUrl,
                    appId: cfg.AppId,
                    privateKeyPem: cfg.PrivateKey,
                    format: "json",
                    version: "1.0",
                    signType: cfg.SignType,
                    alipayPulicKey: cfg.AlipayPublicKey,
                    charset: cfg.Charset?? charset1
                );
            });
            #endregion
            //AutoMap
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            // 1. 注册 JwtBearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:SecretKey"]))
                };
            });
            // 1. 允许 5124 + 5173（前端 devServer）
            var corsUrls = new[] { "http://localhost:5124", "http://localhost:5173" };

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsUrls)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();   // 若带 Authorization 头
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            
            app.Run();
        }
    }
}
