using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Sys
{
    internal class MenuPermissionEntityConfig : BaseEntityConfig<MenuPermissionEntity>
    {
        public override void Configure(EntityTypeBuilder<MenuPermissionEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("MenuPermission");
            // 明确指定复合主键
            //builder.HasKey(mp => new { mp.MenuId, mp.PermissionId });

            // 配置外键关系 - 单向导航：MenuPermission -> SysMenu
            builder.HasOne(mp => mp.Menu)
                .WithMany(mp=>mp.MenuPermissions) // 单向导航，不在SysMenu中配置导航属性
                .HasForeignKey(mp => mp.MenuId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除菜单时删除所有权限

            // 配置外键关系 - 单向导航：MenuPermission -> Permission
            builder.HasOne(mp => mp.Permission)
                .WithMany(mp=>mp.MenuPermissions) // 单向导航，不在Permission中配置导航属性
                .HasForeignKey(mp => mp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除权限时删除所有关联

            // 配置复合唯一索引
            builder.HasIndex(mp => new { mp.MenuId, mp.PermissionId })
                .IsUnique()
                .HasDatabaseName("IX_MenuPermission_Menu_Permission");

            // 配置单独索引
            builder.HasIndex(mp => mp.MenuId)
                .HasDatabaseName("IX_MenuPermission_MenuId");

            builder.HasIndex(mp => mp.PermissionId)
                .HasDatabaseName("IX_MenuPermission_PermissionId");
        }
    }
}
