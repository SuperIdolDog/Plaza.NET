using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.User
{
    /// <summary>
    /// 系统通知
    /// </summary>
    public class NotificationEntity : BaseEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 类型
        /// </summary>
        public int NotificationTypeItemId { get; set; }

        public virtual DictionaryItemEntity NotificationTypeItem { get; set; } = null!;

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Link { get; set; } = null!;

        /// <summary>
        /// 接收用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 接收用户
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;

        /// <summary>
        /// 发布者ID
        /// </summary>
        public int PublisherId { get; set; }

        /// <summary>
        /// 发布者
        /// </summary>
        public virtual UserEntity Publisher { get; set; } = null!;
    }
}
