using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.Store;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels;

using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Store
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IStoreService _storeService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IFloorService _floorService;

        private readonly IPlazaService _plazaService;
        public ProductController(
            IProductService productService,
            IProductTypeService productTypeService,
            IStoreService storeService,
            IImageUploadService imageUploadService,
            IPlazaService plazaService,
            IFloorService floorService)
        {
            _productService = productService;
            _productTypeService = productTypeService;
            _storeService = storeService;
            _imageUploadService = imageUploadService;
            _floorService = floorService;
            _plazaService = plazaService;

        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, string uploadType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "请选择有效的图片文件" });

                var imagePath = await _imageUploadService.UploadImageAsync(file, uploadType);
                return Json(new { success = true, imageUrl = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return Json(new { success = false, message = "图片路径不能为空" });

                var isDeleted = _imageUploadService.DeleteImageAsync(imageUrl);
                return isDeleted ?
                    Json(new { success = true }) :
                    Json(new { success = false, message = "删除图片失败" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                var stores = await _storeService.GetManyByAsync(s => s.FloorId == floorId && s.Floor.PlazaId == plazaId);
                return Json(stores);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductTypesByStoreId(int plazaId, int floorId,int storeId)
        {
            var productTypes = await _productTypeService.GetManyByAsync(pt => pt.Store.FloorId == floorId && pt.Store.Floor.PlazaId == plazaId&& pt.StoreId == storeId);
            return Json(productTypes);
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId,int? floorId,int? storeId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            IEnumerable<StoreEntity> stores = new List<StoreEntity>();
            IEnumerable<ProductTypeEntity> productTypes = new List<ProductTypeEntity>();
            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
                if (floorId.GetValueOrDefault() != 0)
                {
                    stores = await _storeService.GetManyByAsync(f => f.FloorId == floorId!.Value);
                    if (storeId.GetValueOrDefault() != 0)
                    {
                        productTypes = await _productTypeService.GetManyByAsync(f => f.StoreId == storeId!.Value);
                    }
                }
            }
            var viewModel = new ProductIndexViewModel
            {
                Plazas = plazas,
                Floors = floors,
                Stores = stores,
                ProductTypes = productTypes
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductData(
     int pageIndex,
     int pageSize,
     int? status,
     int? productTypeId,
     int? storeId,
     int? plazaId,
     int? floorId,
     string keyword,
     string productName
 )
        {

                Expression<Func<ProductEntity, bool>> predicate = p =>
                    (string.IsNullOrWhiteSpace(keyword) ||
                     p.Name.Contains(keyword) ||
                     p.Description.Contains(keyword) ||
                     (p.Store != null && p.Store.Name.Contains(keyword)) ||
                     (p.ProductType != null && p.ProductType.Name.Contains(keyword)) ||
                     (p.Store != null && p.Store.Floor != null && p.Store.Floor.Name.Contains(keyword)) ||
                     (p.Store != null && p.Store.Floor != null && p.Store.Floor.Plaza != null && p.Store.Floor.Plaza.Name.Contains(keyword))) &&
                    (string.IsNullOrWhiteSpace(productName) || (p.Name != null && p.Name.Contains(productName))) &&
                    (!status.HasValue || p.IsDeleted == (status.Value == 1)) &&
                    (!productTypeId.HasValue || p.ProductTypeId == productTypeId.Value) &&
                    (!storeId.HasValue || p.StoreId == storeId.Value) &&
                    (!plazaId.HasValue || (p.Store != null && p.Store.Floor != null && p.Store.Floor.PlazaId == plazaId.Value)) &&
                    (!floorId.HasValue || (p.Store != null && p.Store.FloorId == floorId.Value));

                var query = await _productService.GetPagedListByAsync(pageIndex, pageSize, predicate,
                    include: p => p.Include(pr => pr.ProductType)
                                  .Include(pr => pr.Store)
                                  .ThenInclude(s => s.Floor)
                                  .ThenInclude(f => f.Plaza));

                var total = await _productService.CountByAsync(predicate);

                var result = query.Select(p => new
                {
                    p.Id,
                    p.Name,

                    p.ImageUrl,
                    p.Description,

                    p.ProductTypeId,
                    productTypeName = p.ProductType?.Name,
                    p.StoreId,
                    storeName = p.Store?.Name,
                    storeFloorName = p.Store?.Floor?.Name,
                    p.Store?.FloorId,
                    floorName=p.Store?.Floor?.Name,
                    p.Store?.Floor.PlazaId,
                    plazaName = p.Store?.Floor?.Plaza?.Name,
                    p.IsDeleted,
                    p.CreateTime,
                    p.UpdateTime,
                });

                return Json(new
                {
                    rows = result,
                    Total = total
                });
            }
        

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEntity product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("商品数据不能为空");
                }

                product.UpdateTime = DateTime.Now;
                var result = await _productService.UpdateAsync(product);

                return result ?
                    Json(new { success = true, message = "更新成功" }) :
                    Json(new { success = false, message = "更新失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductEntity product)
        {
            try
            {
                var result = await _productService.CreateAsync(product);
                return result ?
                    Json(new { success = true, message = "添加成功" }) :
                    Json(new { success = false, message = "添加失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, ProductEntity product)
        {
            var result = await _productService.DeleteAsync(product);
            return result ?
                Json(new { success = true, message = "删除成功" }) :
                Json(new { success = false, message = "删除失败" });
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

                var result = await _productService.DeleteRangeAsync(ids);
                return result ?
                    Json(new { success = true, message = "删除成功" }) :
                    Json(new { success = false, message = "删除失败" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

    }

  
}
