using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLogEntity : BaseEntity
    {
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登出时间
        /// </summary>
        public DateTime? LogoutTime { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; } = null!;

        /// <summary>
        /// 设备信息
        /// </summary>
        public string DeviceInfo { get; set; } = null!;

        /// <summary>
        /// 状态：0-失败, 1-成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string? FailureReason { get; set; } 

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
