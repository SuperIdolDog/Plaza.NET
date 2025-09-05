using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.User
{
    public class AddressEntity : BaseEntity
    {
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; } = null!;
        /// <summary>
        /// 收货人
        /// </summary>
        public string Name { get; set; } = null!;      // 收货人
        /// <summary>
        /// 练习方式
        /// </summary>
        public string Phone { get; set; } = null!;     // 手机号
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; } = null!;
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; } = null!;
        /// <summary>
        /// 区
        /// </summary>
        public string County { get; set; } = null!;
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Detail { get; set; } = null!;
        /// <summary>
        /// 标签
        /// </summary>
        public string? Label { get; set; }             // 家/公司/学校
        /// <summary>
        /// 是否为默认地址
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
