using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.User
{
    /// <summary>
    /// 用户反馈
    /// </summary>
    public class UserFeedbackEntity : BaseEntity
    {

        /// <summary>
        /// 反馈图片
        /// </summary>
        public string Imageurl { get; set; } = null!;
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; } = null!;

        /// <summary>
        /// 回复内容
        /// </summary>
        public string? ReplyContent { get; set; } 

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;

        /// <summary>
        /// 回复者ID
        /// </summary>
        public int? RepliedById { get; set; }

        /// <summary>
        /// 回复者
        /// </summary>
        public virtual UserEntity RepliedBy { get; set; } = null!;
    }
}
