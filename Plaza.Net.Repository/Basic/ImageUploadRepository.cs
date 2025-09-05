using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Plaza.Net.IRepository.Basic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Basic
{
    public class ImageUploadRepository : IImageUploadRepository
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ImageUploadRepository(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public bool DeleteImageAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                var absolutePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", filePath.TrimStart('/'));

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string uploadType = "default")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("无效的图片文件");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("只支持上传jpg, jpeg, png, gif, bmp格式的图片");

            var maxFileSize = 2 * 1024 * 1024; // 2MB
            if (file.Length > maxFileSize)
                throw new ArgumentException("图片大小不能超过2MB");

            try
            {
                // 修改上传目录为 wwwroot/images/{uploadType}
                var uploadFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images", uploadType);
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 返回相对路径，例如 "images/default/filename.png"
                return Path.Combine("images", uploadType, fileName);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"图片上传失败: {ex.Message}");
                throw new Exception($"图片上传失败: {ex.Message}");
            }
        }
    }
}
