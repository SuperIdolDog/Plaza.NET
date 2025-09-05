using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 停车位
    /// </summary>
    public class ParkingSpotEntity : BaseEntity
    {
        /// <summary>
        /// 停车位名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 状态：0-空闲, 1-占用, 2-维修中
        /// </summary>
        public int ParkingSpotItemId { get; set; }
        public virtual DictionaryItemEntity ParkingSpotItem { get; set; } = null!;

        /// <summary>
        /// 所属停车场区域ID
        /// </summary>
        public int ParkingAreaId { get; set; }

        /// <summary>
        /// 所属停车场区域
        /// </summary>
        public virtual ParkingAreaEntity ParkingArea { get; set; } = null!;
    }
}
