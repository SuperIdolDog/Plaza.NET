using Microsoft.AspNetCore.Identity;
using Plaza.Net.Model.Entities.Sys;


namespace Plaza.Net.Model.Entities.Basic
{
    /// <summary>
    /// 用户身份信息
    /// </summary>
    public class UserRoleEntity : IdentityRole<int>,IBaseEntity
    {
        /// <summary>
        /// 用户角色信息描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string? Code { get; set; } 
        /// <summary>
        /// 软删除
        /// </summary>
        public bool IsDeleted { get; set; }=false;

        /// <summary>
        /// 权限集合（一个角色可以拥有多个权限）
        /// </summary>
        public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();

        /// <summary>
        /// 用户集合（一个角色可以属于多个用户）
        /// </summary>
        public virtual ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();


    }
}

