using Microsoft.AspNetCore.Http;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Basic
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IImageUploadRepository _repository;
        public ImageUploadService(IImageUploadRepository repository)
        {
            _repository = repository;
        }
        public  bool DeleteImageAsync(string filePath)
        {
            return _repository.DeleteImageAsync(filePath);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string uploadType = "default")
        {
            return await _repository.UploadImageAsync(file, uploadType);
        }
    }
}
