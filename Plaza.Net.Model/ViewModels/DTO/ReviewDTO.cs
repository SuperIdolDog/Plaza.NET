using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class ReviewDTO
    {
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserAvatar { get; set; }
        /// <summary>
        /// 评分（1-5星）
        /// </summary>
        public string Rating { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        // <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 评价创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public string SkusSpecNames { get; set; } = string.Empty;

        /// <summary>
        /// 商品规格值详情
        /// </summary>
        public List<ProductSpecValueDto> SkusSpecValues { get; set; } = new List<ProductSpecValueDto>();

    }
}
