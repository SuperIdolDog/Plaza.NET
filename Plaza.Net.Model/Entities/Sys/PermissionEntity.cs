using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class PermissionEntity : BaseEntity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 所属角色
        /// </summary>
        public int RoleId { get; set; }
        public virtual UserRoleEntity Role { get; set; } = null!;
        public virtual ICollection<MenuPermissionEntity> MenuPermissions { get; set; } = new List<MenuPermissionEntity>();
    }
}


