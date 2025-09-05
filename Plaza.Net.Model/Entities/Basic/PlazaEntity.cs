using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 广场信息
    /// </summary>
    public class PlazaEntity : BaseEntity
    {
        /// <summary>
        /// 广场名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 广场地址
        /// </summary>
        public string Address { get; set; } = null!;

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 所属楼层集合
        /// </summary>
        public virtual ICollection<FloorEntity> Floors { get; set; } = new List<FloorEntity>();
    }
}
