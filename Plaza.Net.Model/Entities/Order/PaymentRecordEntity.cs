using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Order
{
    /// <summary>
    /// 支付记录
    /// </summary>
    public class PaymentRecordEntity : BaseEntity
    {
        /// <summary>
        /// 支付方式：0-现金, 1-支付宝, 2-微信, 3-银行卡
        /// </summary>
        public int PaymentMethodItemId { get; set; }
        /// <summary>
        /// 支付类型集合
        /// </summary>
        public virtual DictionaryItemEntity PaymentMethodItem { get; set; } = null!;
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PaymentTime { get; set; }

        /// <summary>
        /// 状态：0-处理中, 1-成功, 2-失败
        /// </summary>
        public int PaystatuItemId { get; set; }
        public virtual DictionaryItemEntity PaystatuItem { get; set; } = null!;
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TransactionId { get; set; } = null!;

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public virtual OrderEntity Order { get; set; } = null!;

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;
    }
}
