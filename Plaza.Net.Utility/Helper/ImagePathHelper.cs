using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Utility.Helper
{
    public static class ImagePathHelper
    {
        private static readonly string _mvcBaseUrl;

        /// <summary>
        /// 静态构造函数，初始化配置
        /// </summary>
        static ImagePathHelper()
        {
            // 构建配置读取器
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 读取配置中的基础URL
            _mvcBaseUrl = configuration["AppSettings:MvcBaseUrl"] ?? "http://localhost:5132/";

            // 确保URL以斜杠结尾，避免拼接问题
            if (!_mvcBaseUrl.EndsWith("/"))
            {
                _mvcBaseUrl += "/";
            }
        }

        /// <summary>
        /// 将数据库中的路径转换为完整图片URL
        /// </summary>
        /// <param name="dbPath">数据库中存储的路径（如：images\store\test.png）</param>
        /// <returns>完整可访问的URL（如：http://localhost:5132/images/store/test.png）</returns>
        public static string ConvertToFullUrl(string dbPath)
        {
            // 处理空路径
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                return string.Empty;
            }

            // 1. 将反斜杠转换为正斜杠（URL必须使用正斜杠）
            string normalizedPath = dbPath.Replace("\\", "/");

            // 2. 拼接基础URL和处理后的路径
            return $"{_mvcBaseUrl}{normalizedPath}";
        }

        /// <summary>
        /// 批量转换图片路径
        /// </summary>
        public static List<string> ConvertToFullUrls(List<string> dbPaths)
        {
            if (dbPaths == null || !dbPaths.Any())
            {
                return new List<string>();
            }

            return dbPaths.Select(ConvertToFullUrl).ToList();
        }
    }
}
