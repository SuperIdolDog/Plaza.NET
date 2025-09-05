using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Device
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public class DeviceTypeEntity : BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 类型描述
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; } = null!;

        /// <summary>
        /// 设备集合
        /// </summary>
        public virtual ICollection<DeviceEntity> Devices { get; set; } = new List<DeviceEntity>();
    }
}
