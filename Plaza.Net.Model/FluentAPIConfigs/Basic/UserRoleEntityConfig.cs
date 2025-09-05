using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class UserRoleEntityConfig : IEntityTypeConfiguration<UserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            builder.ToTable("UserRole");
            // 配置角色描述属性
            builder.Property(ur => ur.Description)
                .HasMaxLength(200);

            // 配置角色代码属性
            builder.Property(ur => ur.Code)
                .HasMaxLength(50);

            // 配置索引
            builder.HasIndex(ur => ur.Code)
                .IsUnique()
                .HasDatabaseName("IX_UserRole_Code");

            builder.HasIndex(ur => ur.IsDeleted)
                .HasDatabaseName("IX_UserRole_IsDeleted");


        }
    }
}
