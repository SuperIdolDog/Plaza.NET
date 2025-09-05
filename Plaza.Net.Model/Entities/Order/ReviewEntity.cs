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
    /// 商品评价
    /// </summary>
    public class ReviewEntity : BaseEntity
    {
        /// <summary>
        /// 评分：1-5星
        /// </summary>
        public int ReviewRatingItemId { get; set; }
        public virtual DictionaryItemEntity ReviewRatingItem { get; set; } = null!;
        /// <summary>
        /// 评价内容
        /// </summary>
        public string Content { get; set; } = null!;
        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; } = null!;
        /// <summary>
        /// 订单项ID
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public virtual OrderItemEntity OrderItem { get; set; } = null!;

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
