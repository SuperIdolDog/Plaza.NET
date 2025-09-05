using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    public class DictionaryEntity:BaseEntity
    {
        /// <summary>
        /// 字典类型名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 字典类型描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 字典详情集合
        /// </summary>
        public virtual ICollection<DictionaryItemEntity> DictionaryItems { get; set; } = new List<DictionaryItemEntity>();
    }
}
