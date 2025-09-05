using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FromBody
{
    public class FMUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = null!;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string? PhoneNumber { get; set; }
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
    }
}
