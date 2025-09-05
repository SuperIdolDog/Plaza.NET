using Plaza.Net.Model.Entities.Basic;

namespace Plaza.Net.Model.Entities.Sys
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class SysMenuEntity : BaseEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// 排序序号
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父菜单
        /// </summary>
        public virtual SysMenuEntity? Parent { get; set; }


        /// <summary>
        /// 子菜单集合
        /// </summary>
        public virtual ICollection<SysMenuEntity> Children { get; set; } = new List<SysMenuEntity>();
        /// <summary>
        /// 访问该菜单所需的权限
        /// </summary>
        public virtual ICollection<MenuPermissionEntity> MenuPermissions { get; set; } = new List<MenuPermissionEntity>();
    }
}
