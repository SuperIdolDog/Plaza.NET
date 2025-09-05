using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    /// <summary>
    /// 商品信息
    /// </summary>
    public class ProductEntity : BaseEntity
    {
        /// <summary>商品名称</summary>
        public string Name { get; set; } = null!;

        /// <summary>商品主图 URL</summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>商品描述/详情</summary>
        public string Description { get; set; } = null!;

        /// <summary>商品类型 ID</summary>
        public int ProductTypeId { get; set; }
        public virtual ProductTypeEntity ProductType { get; set; } = null!;

        /// <summary>店铺 ID</summary>
        public int StoreId { get; set; }
        public virtual StoreEntity Store { get; set; } = null!;

        /// <summary>该商品下的所有 SKU</summary>
        public virtual ICollection<ProductSkuEntity> Skus { get; set; } = new List<ProductSkuEntity>();

    }
}
