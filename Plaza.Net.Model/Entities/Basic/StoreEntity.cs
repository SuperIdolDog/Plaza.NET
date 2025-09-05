using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 店铺信息
    /// </summary>
    public class StoreEntity : BaseEntity
    {
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImageUrl { get; set; } = null!;
        /// <summary>
        /// 轮播图图片URL
        /// </summary>
        public string SwiperImageUrl { get; set; } = null!;

        /// <summary>
        /// 位置
        /// </summary>
        public string Location { get; set; } = null!;

        /// <summary>
        /// 营业时间
        /// </summary>
        public string BusinessHours { get; set; } = null!;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; } = null!;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 评分
        /// </summary>
        public double Rating { get; set; }
        /// <summary>
        /// 是否营业
        /// </summary>
        public bool IsOperating { get; set; }
        /// <summary>
        /// 店铺类型ID
        /// </summary>
        public int StoreTypeId { get; set; }

        /// <summary>
        /// 店铺类型
        /// </summary>
        public virtual StoreTypeEntity StoreType { get; set; } = null!;

        /// <summary>
        /// 楼层ID
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// 楼层
        /// </summary>
        public virtual FloorEntity Floor { get; set; } = null!;

        /// <summary>
        /// 店长ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 店主
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;
        /// <summary>
        /// 店铺商品集合
        /// </summary>
        public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}
