using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    /// <summary>
    /// 商品类型
    /// </summary>
    public class ProductTypeEntity : BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 商店的物品类型
        /// </summary>
        public int StoreId { get; set; }
        public virtual StoreEntity Store { get; set; } = null!;
        /// <summary>
        /// 单位：0-个, 1-件, 2-千克等
        /// </summary>
        public int ProductTypeUnitItemId { get; set; }


        public virtual DictionaryItemEntity ProductTypeUnitItem { get; set; } = null!;
        /// <summary>
        /// 商品集合
        /// </summary>
        public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}
