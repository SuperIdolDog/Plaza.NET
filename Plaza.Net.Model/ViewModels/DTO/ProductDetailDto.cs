using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }
        public string BarCode { get; set; } = string.Empty;

        public List<ProductSpecDto> Specs { get; set; } = new List<ProductSpecDto>();
        public List<ProductSkuDto> Skus { get; set; } = new List<ProductSkuDto>();

    }

    public class ProductSpecDto
    {
        public int SpecId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public List<ProductSpecValueDto> Values { get; set; } = new List<ProductSpecValueDto>();
    }


    public class ProductSkuDto
    {
        public int SkuId { get; set; }
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }
        public List<SkuSpecValueDto> SkusSpecValues { get; set; } = new List<SkuSpecValueDto>();
    }
    public class ProductSpecValueDto
    {
        public int ValueId { get; set; }
        public string ValueName { get; set; } = string.Empty;
    }
    public class SkuSpecValueDto     // 仅用于 SKU 内部
    {
        public int SpecId { get; set; }  
        public int ValueId { get; set; }   // 规格值 ID
    }
}
