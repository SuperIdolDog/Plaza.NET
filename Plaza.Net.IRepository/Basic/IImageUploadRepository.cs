

using Microsoft.AspNetCore.Http;

namespace Plaza.Net.IRepository.Basic
{
    public interface IImageUploadRepository
    {
        Task<string> UploadImageAsync(IFormFile file, string uploadType = "default");
        bool DeleteImageAsync(string filePath);
    }
}
