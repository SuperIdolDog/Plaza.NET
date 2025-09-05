using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Store;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Store
{
    public class ProductTypeController : Controller
    {
        private readonly IProductTypeService _productTypeService;
        private readonly IStoreService _storeService;
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly EFDbContext _dbContext;

        public ProductTypeController(
             IProductTypeService productTypeService,
             IStoreService storeService,
             IPlazaService plazaService,
             IFloorService floorService,
             IDictionaryItemService dictionaryItemService,
             EFDbContext dbContext)
        {
            _productTypeService = productTypeService;
            _storeService = storeService;
            _plazaService = plazaService;
            _floorService = floorService;
            _dictionaryItemService = dictionaryItemService;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetFloorsByPlazaId(int plazaId)
        {
            var floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId);
            return Json(floors);
        }
        [HttpGet]
        public async Task<IActionResult> GetStoresByFloorId(int plazaId, int floorId)
        {
            try
            {
                var stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId && s.Floor.PlazaId==plazaId);
                return Json(stores);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId, int? floorId, int? storeId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<StoreEntity> stores = new List<StoreEntity>();
            var units = await _dbContext.Dictionary
                .Where(d => d.Name == "商品计量单位")
                .Include(di => di.DictionaryItems)
                .SelectMany(di => di.DictionaryItems)
                .ToListAsync();

            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
                if (floorId.GetValueOrDefault() != 0)
                {
                    stores = await _storeService.GetManyByAsync(f => f.FloorId == floorId!.Value);
                }
            }

            var viewModel = new ProductTypeIndexViewModel
            {
                Plazas = plazas,
                Floors = floors,
                Stores = stores,
                Units = units
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductTypeData(
            int pageIndex,
            int pageSize,
            string keyword,
            string name,
            int? unitId,
            int? plazaId,
            int? floorId,
            int? storeId,
            int? status)
        {
            Expression<Func<ProductTypeEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 (p.ProductTypeUnitItem != null && p.ProductTypeUnitItem.Label.Contains(keyword)) ||
                 (p.Store != null && p.Store.Name.Contains(keyword)) ||
                 (p.Store != null && p.Store.Floor != null && p.Store.Floor.Name.Contains(keyword)) ||
                 (p.Store != null && p.Store.Floor != null && p.Store.Floor.Plaza != null && p.Store.Floor.Plaza.Name.Contains(keyword))) &&
                (string.IsNullOrWhiteSpace(name) || p.Name.Contains(name)) &&
                (!unitId.HasValue || p.ProductTypeUnitItemId == unitId) &&
                (!plazaId.HasValue || (p.Store != null && p.Store.Floor != null && p.Store.Floor.PlazaId == plazaId.Value)) &&
                (!floorId.HasValue || (p.Store != null && p.Store.FloorId == floorId.Value)) &&
                (!storeId.HasValue || p.StoreId == storeId.Value) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1));

            var query = await _productTypeService.GetPagedListByAsync(pageIndex, pageSize, predicate,
                include: p => p.Include(pt => pt.ProductTypeUnitItem)
                              .Include(pt => pt.Store)
                              .ThenInclude(s => s.Floor)
                              .ThenInclude(f => f.Plaza));

            var total = await _productTypeService.CountByAsync(predicate);

            var result = query.Select(p => new
            {
                p.Id,
                p.Name,
                p.StoreId,
                storeName = p.Store?.Name,
                p.ProductTypeUnitItemId,
                unitName = p.ProductTypeUnitItem?.Label,
                p.IsDeleted,
                p.CreateTime,
                p.UpdateTime,
                plazaId = p.Store?.Floor?.PlazaId,
                PlazaName = p.Store?.Floor?.Plaza?.Name,
                floorId = p.Store?.FloorId,
                floorName = p.Store?.Floor?.Name
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductTypeEntity productType)
        {
            try
            {
                if (productType == null)
                {
                    return BadRequest("商品类型数据不能为空");
                }

                var result = await _productTypeService.CreateAsync(productType);

                if (result)
                {
                    return Json(new { success = true, message = "添加成功" });
                }
                else
                {
                    return Json(new { success = false, message = "添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductTypeEntity productType)
        {
            try
            {
                if (productType == null)
                {
                    return BadRequest("商品类型数据不能为空");
                }

                productType.UpdateTime = DateTime.Now;
                var result = await _productTypeService.UpdateAsync(productType);

                if (result)
                {
                    return Json(new { success = true, message = "更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, ProductTypeEntity productType)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _productTypeService.DeleteAsync(productType);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _productTypeService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }


    }
}
