using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Sys;


namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 停车场区域
    /// </summary>
    public class ParkingAreaEntity : BaseEntity
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 所属楼层ID
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// 所属楼层
        /// </summary>
        public virtual FloorEntity Floor { get; set; } = null!;
        /// <summary>
        /// 停车区域类型
        /// </summary>
        public int ParkingAreaItemId { get; set; }
        public virtual DictionaryItemEntity ParkingAreaItem { get; set; } = null!;
        public virtual ICollection<ParkingSpotEntity> ParkingSpot { get; set; } = new List<ParkingSpotEntity>();
    }

}

