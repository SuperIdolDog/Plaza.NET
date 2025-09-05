using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Order;
using Plaza.Net.IServices.Store;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Utility.Helper;
using System.Linq.Expressions;
using System.Threading;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Plaza.Net.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly IReviewService _reviewService;
        private readonly IProductService _productService;
        private readonly EFDbContext _dbContext;
        public GoodController(IProductTypeService productTypeService, EFDbContext dbContext, IProductService productService, IReviewService reviewService)
        {
            _productTypeService = productTypeService;
            _dbContext = dbContext;
            _productService = productService;
            _reviewService = reviewService;

        }
        [HttpGet("store/{storeId}/productTypes")]
        public async Task<IActionResult> GetProductTypesByStoreId(int storeId)
        {

            var goodtypes = await _productTypeService.GetManyByAsync(f => f.StoreId == storeId);

            return Ok(goodtypes.Select(p => new
            {
                p.Id,
                p.Name,
            }));
        }
        [HttpGet("store/{storeId}/products")]
        public async Task<IActionResult> GetProducts(int storeId, [FromQuery] int? productTypeId)
        {
            Expression<Func<ProductEntity, bool>> predicate = p =>
        p.StoreId == storeId &&
        (!productTypeId.HasValue || p.ProductTypeId == productTypeId.Value);

            var products = await _productService.GetManyByAsync(predicate);

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Name,
                Cover = ImagePathHelper.ConvertToFullUrl(p.ImageUrl),
                p.ProductTypeId,
            }));
        }
        [HttpGet("store/{productId}/productDetails")]
        public async Task<IActionResult> GetProductDetails(int productId)
        {

            var product = await _productService.GetOneByIdAsync(productId);
            var sku = product.Skus
                             .Where(s => s.IsEnabled && !s.IsDeleted)
                             .OrderBy(s => s.Price)
                             .FirstOrDefault();

            if (sku == null)
                return BadRequest("商品无可用 SKU");

            // 2. 构造规格树
            var specs = product.Skus
                .SelectMany(s => s.SpecValueMappings)
                .GroupBy(sv => new { sv.ProductSpecValue.Spec.Id, sv.ProductSpecValue.Spec.Name })
                .Select(g => new ProductSpecDto
                {
                    SpecId = g.Key.Id,
                    SpecName = g.Key.Name,
                    Values = g.Select(v => new ProductSpecValueDto
                    {
                        ValueId = v.ProductSpecValue.Id,
                        ValueName = v.ProductSpecValue.Value
                    }).Distinct().ToList()
                })
                .OrderBy(s => s.SpecId)
                .ToList();

            // 3. 构造 DTO
            // 在构造 DTO 时补上 Skus
            var dto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = ImagePathHelper.ConvertToFullUrl(product.ImageUrl),
                Description = product.Description,
                Price = sku.Price,
                StockQuantity = sku.StockQuantity,
                BarCode = sku.BarCode ?? string.Empty,
                Specs = specs,
                Skus = product.Skus
                    .Where(s => s.IsEnabled && !s.IsDeleted)
                    .Select(s => new ProductSkuDto
                    {
                        SkuId = s.Id,
                        Price = s.Price,
                        StockQuantity = s.StockQuantity,
                        SkusSpecValues = s.SpecValueMappings
                            .Select(m => new SkuSpecValueDto
                            {
                                SpecId = m.ProductSpecValue.Spec.Id,
                                ValueId = m.ProductSpecValue.Id
                            }).ToList()
                    }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("store/{productId}/ProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId,
         int pageIndex = 1,
        int pageSize = 2)
        {
            Expression<Func<ReviewEntity, bool>> predicate = p => p.OrderItem.ProductSku.ProductId == productId;

            // 构建动态条件表达式
            var review = await _reviewService.GetPagedListByAsync( pageIndex, pageSize,predicate,
                include: r => r.Include(o => o.ReviewRatingItem)
                             .Include(oi=>oi.OrderItem)
                             .ThenInclude(p => p.ProductSku)
                             .ThenInclude(p=>p.Product));

            var dtos = review.Select(r => new ReviewDTO
            {
                UserAvatar=r.User.AvatarUrl,
                Rating = r.ReviewRatingItem.Value,
                Content = r.Content,
                UserName = r.User.UserName,
                ProductName = r.OrderItem.ProductSku.Product.Name,
                CreateTime = r.CreateTime,
                // 拼接规格字符串
                SkusSpecNames = string.Join("，", r.OrderItem.ProductSku.SpecValueMappings
                .Select(svm => svm.ProductSpecValue.Value)),
                // 详细规格列表
                SkusSpecValues = r.OrderItem.ProductSku.SpecValueMappings
                .Select(svm => new ProductSpecValueDto
                {
                    ValueName = svm.ProductSpecValue.Value
                }).ToList()
            }).OrderByDescending(r => r.CreateTime).ToList();


            return Ok(dtos);
        }
    }
}
