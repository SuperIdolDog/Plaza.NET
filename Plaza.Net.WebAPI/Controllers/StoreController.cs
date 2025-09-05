using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Utility.Helper;
using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Plaza.Net.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        private readonly IStoreTypeService _storeTypeService;
        private readonly IStoreService _storeService;
        private readonly EFDbContext _dbContext;

        public StoreController(IStoreTypeService storeTypeService, IStoreService storeService, EFDbContext dbContext)
        {
            _storeTypeService = storeTypeService;
            _storeService = storeService;
            _dbContext = dbContext;
        }
        // GET /api/storetypes
        [HttpGet("storetypes")]
        public async Task<IActionResult> GetStoreTypes()
        {
            var types = await _storeTypeService.GetAllAsync();
            return Ok(types.Select(t => new
            {
                t.Id,
                t.Name
            }));

        }


        [HttpGet("stores")]
        public async Task<IActionResult> GetStores(
        [FromQuery] int plazaId,
        [FromQuery] int? floorId,
        [FromQuery] int? storeTypeId,
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10
             )
        {



            Expression<Func<StoreEntity, bool>> predicate = p =>
   (string.IsNullOrWhiteSpace(keyword) || p.Name.Contains(keyword)) &&
   (p.Floor != null && p.Floor.PlazaId == plazaId) &&
   (storeTypeId == null || p.StoreTypeId == storeTypeId) &&
   (floorId == null || p.FloorId == floorId.Value);

            // 构建动态条件表达式
            var store = await _storeService.GetPagedListByAsync(
               pageIndex,
                pageSize,
               predicate,
               include: p => p.Include(d => d.StoreType)
                             .Include(d => d.Floor)
                             .Include(f => f.Floor.Plaza));



            var dtos = store.Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ImageURL= ImagePathHelper.ConvertToFullUrl(s.ImageUrl),
                Location =s.Location,
                StoreTypeId = s.StoreTypeId,
                StoreTypeName = s.StoreType.Name,
                FloorId = s.Floor.Id,
                FloorName = s.Floor.Name,
                PlazaId = s.Floor.Plaza.Id,
                PlazaName = s.Floor.Plaza.Name
                
            }).ToList();

            return Ok(dtos);
        }


        [HttpGet("storeInfo/{storeId}")]
        public async Task<IActionResult> GetStoreInfoById(int storeId)
        {
            var store = await _storeService.GetOneByIdAsync(storeId);


            var storeResult = new
            {
                store.Id,
                store.Name,
                ImageUrl = ImagePathHelper.ConvertToFullUrl(store.ImageUrl),
                store.Location,
                store.Description,
                store.BusinessHours,
                store.Contact
            };
            return Ok(storeResult);
        }
    }
}
