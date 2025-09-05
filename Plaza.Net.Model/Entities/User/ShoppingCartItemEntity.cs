using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.User
{
    /// <summary>
    /// 购物车项
    /// </summary>
    public class ShoppingCartItemEntity : BaseEntity
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;

        /// <summary>
        /// 商品ID
        /// </summary>
        public int ProductSkuId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public virtual ProductSkuEntity ProductSku { get; set; } = null!;
    }
}
