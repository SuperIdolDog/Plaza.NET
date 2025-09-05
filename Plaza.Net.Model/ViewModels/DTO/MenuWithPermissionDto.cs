using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class MenuWithPermissionDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 菜单排序序号
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否有权限访问该菜单
        /// </summary>
        public bool HasPermission { get; set; }

        /// <summary>
        /// 子菜单集合
        /// </summary>
        public List<MenuWithPermissionDto> Children { get; set; } = new List<MenuWithPermissionDto>();
    }
}
