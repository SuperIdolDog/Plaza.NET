using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    /// <summary>
    /// 字典详情
    /// </summary>
    public class DictionaryItemEntity:BaseEntity
    {
        /// <summary>
        /// 字典项标签
        /// </summary>
        public string Label { get; set; } = null!;

        /// <summary>
        /// 字典项值
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// 字典项描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否默认项
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 字典类型ID
        /// </summary>
        public int DictionaryId { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public virtual DictionaryEntity Dictionary { get; set; } = null!;
    }
}
