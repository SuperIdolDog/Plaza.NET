using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OperationLogEntity : BaseEntity
    {
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; } = null!;

        /// <summary>
        /// 模块
        /// </summary>
        public string Module { get; set; } = null!;

        /// <summary>
        /// 目标
        /// </summary>
        public string Target { get; set; } = null!;

        /// <summary>
        /// 详情
        /// </summary>
        public string Details { get; set; } = null!;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; } = null!;

        /// <summary>
        /// 状态：0-失败, 1-成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 执行时间(毫秒)
        /// </summary>
        public int ExecutionTime { get; set; }

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
