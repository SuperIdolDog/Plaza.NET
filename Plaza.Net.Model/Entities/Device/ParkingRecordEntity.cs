using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Device
{
    /// <summary>
    /// 车辆进出记录
    /// </summary>
    public class ParkingRecordEntity : BaseEntity
    {
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string PlateNumber { get; set; } = null!;
        /// <summary>
        /// 车牌识别图片
        /// </summary>
        public string PlateImageUrl { get; set; } = null!;
        /// <summary>
        /// 进入时间
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// 离开时间
        /// </summary>
        public DateTime? ExitTime { get; set; }

        /// <summary>
        /// 停车费用
        /// </summary>
        public decimal ParkingFee { get; set; }
        /// <summary>
        /// 是否支付
        /// </summary>
        public bool IsPaid { get; set; }
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
