using Plaza.Net.Model.Entities.Store;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plaza.Net.Model.Entities.Order
{
    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItemEntity : BaseEntity
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价（下单时的商品价格）
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public virtual OrderEntity Order { get; set; } = null!;

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
