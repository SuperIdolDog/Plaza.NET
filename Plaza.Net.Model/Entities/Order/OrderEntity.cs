using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.Sys;

namespace Plaza.Net.Model.Entities.Order
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderEntity : BaseEntity
    {
        /// <summary>
        /// 总金额（由订单项计算得出）
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        ///  0-自提 1-配送
        /// </summary>
        public int DeliveryType { get; set; } 
        /// <summary>
        /// 状态：0-待支付, 1-已支付, 2-已发货, 3-已完成, 4-已取消
        /// </summary>
        public int OrderStatuItemId { get; set; }

        public virtual DictionaryItemEntity OrderStatuItem { get; set; } = null!;

        /// <summary>
        /// 配送地址
        /// </summary>
        public string ShippingAddress { get; set; } = null!;
        /// <summary>
        /// 自提日期
        /// </summary>
        public DateTime? PickUpDate { get; set; }
        /// <summary>
        /// 商店id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 商店
        /// </summary>
        public virtual StoreEntity Store { get; set; } = null!;
        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public virtual UserEntity Customer { get; set; } = null!;

        /// <summary>
        /// 员工ID（处理订单的员工）
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// 员工
        /// </summary>
        public virtual EmployeeEntity Employee { get; set; } = null!;

        /// <summary>
        /// 订单项集合
        /// </summary>
        public virtual ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();

        /// <summary>
        /// 支付集合
        /// </summary>
        public virtual ICollection<PaymentRecordEntity> PaymentRecords { get; set; } = new List<PaymentRecordEntity>();
    }
}
