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
    /// 楼层实体类
    /// </summary>
    public class FloorEntity : BaseEntity
    {
        /// <summary>
        /// 楼层名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 楼层描述
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 所属广场ID
        /// </summary>
        public int PlazaId { get; set; }

        /// <summary>
        /// 所属广场
        /// </summary>
        public virtual PlazaEntity Plaza { get; set; } = null!;


        /// <summary>
        /// 楼层类型
        /// </summary>
        public int FloorItemId { get; set; }
        public virtual DictionaryItemEntity? FloorItem { get; set; }

        /// <summary>
        /// 停车区域集合
        /// </summary>
        public virtual ICollection<ParkingAreaEntity> ParkingAreas { get; set; } = new List<ParkingAreaEntity>();
    }
}
