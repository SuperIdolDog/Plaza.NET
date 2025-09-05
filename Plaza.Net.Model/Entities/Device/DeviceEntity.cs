using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Device
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceEntity : BaseEntity
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 设备位置
        /// </summary>
        public string Location { get; set; } = null!;

        /// <summary>
        /// 状态：0-离线, 1-在线, 2-故障
        /// </summary>
        public int DeviceStatusItemId { get; set; }
        public virtual DictionaryItemEntity DeviceStatusItem { get; set; } = null!;

        /// <summary>
        /// 最后维护日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public virtual DeviceTypeEntity DeviceType { get; set; } = null!;

        /// <summary>
        /// 所属楼层ID
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// 所属楼层
        /// </summary>
        public virtual FloorEntity Floor { get; set; } = null!;
    }
}
