using Microsoft.AspNetCore.Identity;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;

namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserEntity : IdentityUser<int>,IBaseEntity
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string? Code { get; set; }

        ///// <summary>
        ///// 三方token
        ///// </summary>
        //public string? OpenId { get; set; }
        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; } = null!;

        /// <summary>
        /// 用户全名
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string? IDNumber { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// 用户是否在线
        /// </summary>
        public int isOnline { get; set; } = 0;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
        /// <summary>
        /// 软删除
        /// </summary>
        public bool IsDeleted { get; set; }=false;
        /// <summary>
        /// 角色ID
        /// </summary>
        public int UserRoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual UserRoleEntity UserRole { get; set; } = null!;
        /// <summary>
        /// 订单集合
        /// </summary>
        public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
        /// <summary>
        /// 接收信息集合
        /// </summary>
        public virtual  ICollection<NotificationEntity> ReceivedNotifications { get;  set; }= new List<NotificationEntity>();
        /// <summary>
        /// 发布信息集合
        /// </summary>
        public virtual ICollection<NotificationEntity> PublishedNotifications { get; set; } = new List<NotificationEntity>();
        /// <summary>
        /// 收货地址合集
        /// </summary>
        public virtual ICollection<AddressEntity> Addresses { get; set; }=new List<AddressEntity>();
    }
}
