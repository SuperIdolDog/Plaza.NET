using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    public class ProductSkuEntity:BaseEntity
    {
        /// <summary>
        /// SKU 名称（例：iPhone 15 Pro 256G 白色钛金属）
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// SKU 编码（商家内部唯一）
        /// </summary>
        public string SkuCode { get; set; } = null!;

        /// <summary>
        /// SKU 条形码（可为空）
        /// </summary>
        public string? BarCode { get; set; }

        /// <summary>
        /// SKU 主图（可覆盖商品主图）
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 售价（留空则继承商品级价格）
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 市场价/划线价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQuantity { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int Sales { get; set; }

        /// <summary>
        /// 重量（g）
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 是否启用 / 上架
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 所属商品 ID
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 所属商品
        /// </summary>
        public virtual ProductEntity Product { get; set; } = null!;

        /// <summary>
        /// 该 SKU 选中的所有规格值
        /// </summary>
        public virtual ICollection<ProductSkuSpecValueEntity> SpecValueMappings { get; set; } = new List<ProductSkuSpecValueEntity>();
    }
}
