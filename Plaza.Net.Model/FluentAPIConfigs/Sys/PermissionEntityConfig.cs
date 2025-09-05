using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Sys
{
    internal class PermissionEntityConfig:BaseEntityConfig<PermissionEntity>
    {
        public override void Configure(EntityTypeBuilder<PermissionEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Permission");
            // 配置权限名称属性
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            // 配置权限描述属性
            builder.Property(p => p.Description)
                .HasMaxLength(200);

            // 配置外键关系 - 单向导航：Permission -> UserRole
            builder.HasOne(p => p.Role)
                 .WithMany(ur => ur.Permissions) // 双向导航，引用UserRole中的Permissions导航属性
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除角色时删除所有权限

            // 配置索引
            builder.HasIndex(p => p.RoleId)
                .HasDatabaseName("IX_Permission_RoleId");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Permission_Name");
        }
    }
}
