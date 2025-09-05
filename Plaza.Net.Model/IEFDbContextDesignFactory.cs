using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Reflection.Emit;

namespace Plaza.Net.Model
{
    internal class IEFDbContextDesignFactory :
        IDesignTimeDbContextFactory<EFDbContext>
    {
        public EFDbContext CreateDbContext(string[] args)
        {
            // 1. 构建配置（从 appsettings.json 读取连接字符串）
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //// 2. 获取数据库连接字符串
            //var connectionString = configuration.GetConnectionString("EFDbContextConnection");

            //// 3. 配置 DbContextOptionsBuilder
            //var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            //optionsBuilder.UseSqlServer(connectionString);

            //// 4. 创建并返回 DbContext 实例
            //return new EFDbContext(optionsBuilder.Options);

            //// 尝试从环境变量中读取连接字符串
            // var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__EFDbContextConnection");

            //if (string.IsNullOrEmpty(connectionString))
            // {
            //     // 如果环境变量不存在，则回退到 appsettings.json
            //     var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //     var mvcOutputDirectory = Path.Combine(baseDirectory, "..", "..", "..", "..", "Plaza.Net.MVCAdmin", "bin", "Debug", "net6.0");

            //     var configuration = new ConfigurationBuilder()
            //         .SetBasePath(mvcOutputDirectory)
            //         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //         .Build();

            //     connectionString = configuration.GetConnectionString("EFDbContextConnection");
            // }

            // if (string.IsNullOrEmpty(connectionString))
            // {
            //     throw new InvalidOperationException("Connection string not found.");
            // }

            // var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            //// optionsBuilder.UseSqlServer(connectionString);


            // return new EFDbContext(optionsBuilder.Options);

            #region MySQL
            // 1. 构建配置
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            // 2. 获取连接字符串
            string? connectionString = configuration.GetConnectionString("EFDbContextConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("未找到连接字符串 'EFDbContextConnection'");
            }

            // 3. 配置 MySQL 选项（正确方式）
            var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();

            // 定义 MySQL 版本（只需要定义一次）
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 39));

            // 基础 MySQL 配置 - 包含版本参数
            optionsBuilder.UseMySql(
                connectionString,
                serverVersion
            );

            // 4. 配置迁移历史表（必须包含版本参数）
            optionsBuilder.UseMySql(
                connectionString,
                serverVersion, // 这里添加了ServerVersion参数
                options => options.MigrationsHistoryTable("__EFMigrationsHistory")
            );

            return new EFDbContext(optionsBuilder.Options);
            #endregion



            //var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            //optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=PlazaDB;User Id=sa;Password=Access123!@#;TrustServerCertificate=true;");
            //return new EFDbContext(optionsBuilder.Options);
            //DbContextOptionsBuilder<EFDbContext> builder = new
            //    DbContextOptionsBuilder<EFDbContext>();
            //// string connStr = Environment.GetEnvironmentVariable("SqlServer");
            //string connStr = "Server=.;Database=PlazaDB;UID=sa;Password=123456";
            //builder.UseSqlServer(connStr);
            //EFDbContext ctx = new EFDbContext(builder.Options);
            //return ctx;
            //return null;
            // 从环境变量中获取连接字符串
            //var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            //var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            //var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            //var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            //var dbName = Environment.GetEnvironmentVariable("DB_NAME");

            //if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass) || string.IsNullOrEmpty(dbName))
            //{
            //    // 如果没有设置环境变量，则回退到 appsettings.json
            //    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //    if (string.IsNullOrEmpty(baseDirectory))
            //    {
            //        baseDirectory = Environment.CurrentDirectory;
            //    }

            //    while (!File.Exists(Path.Combine(baseDirectory, "appsettings.json")) && !string.IsNullOrEmpty(baseDirectory))
            //    {
            //        var parentDirectory = Directory.GetParent(baseDirectory)?.FullName;
            //        if (string.IsNullOrEmpty(parentDirectory))
            //        {
            //            break;
            //        }
            //        baseDirectory = parentDirectory;
            //    }

            //    if (string.IsNullOrEmpty(baseDirectory) || !File.Exists(Path.Combine(baseDirectory, "appsettings.json")))
            //    {
            //        throw new FileNotFoundException("appsettings.json not found.");
            //    }

            //    var configuration = new ConfigurationBuilder()
            //        .SetBasePath(baseDirectory)
            //        .AddJsonFile("appsettings.json")
            //        .Build();

            //    var connStr = configuration.GetConnectionString("SqlServer");
            //    if (string.IsNullOrEmpty(connStr))
            //    {
            //        throw new InvalidOperationException("Connection string 'SqlServer' not found in appsettings.json.");
            //    }

            //    var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            //    optionsBuilder.UseSqlServer(connStr);

            //    return new EFDbContext(optionsBuilder.Options);
            //}
            //else
            //{
            //    // 使用环境变量构建连接字符串
            //    var connStr = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPass};Encrypt=False;";
            //    var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            //    optionsBuilder.UseSqlServer(connStr);

            //    return new EFDbContext(optionsBuilder.Options);
            //}

        }
    }
}

