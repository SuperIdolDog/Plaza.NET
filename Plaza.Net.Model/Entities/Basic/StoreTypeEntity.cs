using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 店铺类型
    /// </summary>
    public class StoreTypeEntity : BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 店铺集合
        /// </summary>
        public virtual ICollection<StoreEntity> Stores { get; set; } = new List<StoreEntity>();
    }
}
