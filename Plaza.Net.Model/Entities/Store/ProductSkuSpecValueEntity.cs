using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    public class ProductSkuSpecValueEntity:BaseEntity
    {
        /// <summary>
        /// SKU ID
        /// </summary>
        public int ProductSkuId { get; set; }

        /// <summary>
        /// SKU
        /// </summary>
        public virtual ProductSkuEntity ProductSku { get; set; } = null!;

        /// <summary>
        /// 规格值 ID
        /// </summary>
        public int ProductSpecValueId { get; set; }

        /// <summary>
        /// 规格值
        /// </summary>
        public virtual ProductSpecValueEntity ProductSpecValue { get; set; } = null!;
    }
}
