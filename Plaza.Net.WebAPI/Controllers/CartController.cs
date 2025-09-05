using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.User;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Services.User;
using Plaza.Net.Utility.Helper;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace Plaza.Net.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IShoppingCartItemService _shoppingCartItemService;
        public CartController(IShoppingCartItemService shoppingCartItemService)
        {
            _shoppingCartItemService = shoppingCartItemService;
        }
        [HttpGet("shopcart")]
        public async Task<IActionResult> GetShoppingcart
            (
            int userid,
            int plazaid,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10
            )
        {
            Expression<Func<ShoppingCartItemEntity, bool>> predicate = p =>
      p.UserId == userid && p.ProductSku.Product.Store.Floor.Plaza.Id == plazaid;

            var shopcart = await _shoppingCartItemService.GetPagedListByAsync(
               pageIndex,
                pageSize,
               predicate,
               include: p => p.Include(s => s.ProductSku)
               .ThenInclude(p => p.Product)
               .ThenInclude(s => s.Store)
               .ThenInclude(f => f.Floor)
               .ThenInclude(p => p.Plaza));

            var grouped = new List<object>();

            foreach (var item in shopcart)
            {
                // 取出店铺信息
                var store = item.ProductSku.Product.Store;

                // 看看这个店铺在结果里有没有
                var storeGroup = grouped
                    .Cast<dynamic>()
                    .FirstOrDefault(g => (int)g.StoreId == store.Id);

                // 如果没有，就新建一个店铺分组
                if (storeGroup == null)
                {
                    storeGroup = new
                    {
                        StoreId = store.Id,
                        StoreName = store.Name,
                        StoreLogo = store.ImageUrl,
                        Items = new List<object>()
                    };
                    grouped.Add(storeGroup);
                }

                // 把购物车条目转成前端需要的数据，塞进 Items
                var specText = string.Join(" / ",
                    item.ProductSku.SpecValueMappings
                                   .Select(m => m.ProductSpecValue.Value));

                var cartDto = new
                {
                    CartId = item.Id,
                    SkuId = item.ProductSku.Id,
                    Name = item.ProductSku.Name,
                    ImageUrl = item.ProductSku.ImageUrl,
                    Price = item.ProductSku.Price,
                    Quantity = item.Quantity,
                    Stock = item.ProductSku.StockQuantity,
                    Selected = item.Selected,
                    SpecText = specText
                };

                // 把条目加到对应店铺的 Items
                ((List<object>)storeGroup.Items).Add(cartDto);
            }
            return Ok(grouped);

        }
        [HttpPost("delcart")]
        public async Task<IActionResult> DelShoppingcart (int id, ShopCartItemDTO dto)
        {
            var shopcart = await _shoppingCartItemService.GetOneByIdAsync(id);
            if (shopcart == null) return NotFound();
            shopcart.IsDeleted = dto.IsDeleted;
            var result = await _shoppingCartItemService.UpdateAsync(shopcart);
            return Ok(result);
        }
    }
}
