


using Microsoft.AspNetCore.Http;

namespace Plaza.Net.IServices.Basic
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string uploadType = "default");
        bool DeleteImageAsync(string filePath);
    }
}
