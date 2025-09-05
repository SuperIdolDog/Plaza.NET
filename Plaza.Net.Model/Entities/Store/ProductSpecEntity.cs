using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    public class ProductSpecEntity:BaseEntity
    {
        /// <summary>
        /// 维度名称（例如：颜色、尺码、内存）
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 前端展示排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 所属商品 ID
        /// </summary>
        public int GoodsId { get; set; }

        /// <summary>
        /// 所属商品
        /// </summary>
        public virtual ProductEntity Products { get; set; } = null!;
    }
}
