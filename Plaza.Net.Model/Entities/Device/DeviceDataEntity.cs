using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Device
{
    /// <summary>
    /// 设备数据
    /// </summary>
    public class DeviceDataEntity : BaseEntity
    {
        /// <summary>
        /// 数值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 单位：0-温度℃, 1-湿度%, 2-分贝等
        /// </summary>
        public int DeviceDataUnitItemId { get; set; }
        public virtual DictionaryItemEntity DeviceDataUnitItem { get; set; } = null!;
        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceEntity Device { get; set; } = null!;
    }
}
