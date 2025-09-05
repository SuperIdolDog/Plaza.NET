using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class UserEntityConfig : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("User");
            // 配置头像URL属性
            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            // 配置用户全名属性
            builder.Property(u => u.FullName)
                .HasMaxLength(50);

            // 配置身份证号属性
            builder.Property(u => u.IDNumber)
                .HasMaxLength(18);

            // 配置外键关系 - 单向导航：User -> UserRole
            builder.HasOne(u => u.UserRole)
                .WithMany(ur => ur.Users) // 双向导航，引用UserRole中的Users导航属性
                .HasForeignKey(u => u.UserRoleId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：有用户时不能删除角色

            // 配置索引
            builder.HasIndex(u => u.UserRoleId)
                .HasDatabaseName("IX_User_UserRoleId");

            builder.HasIndex(u => u.RegisterDate)
                .HasDatabaseName("IX_User_RegisterDate");

            builder.HasIndex(u => u.LastLoginDate)
                .HasDatabaseName("IX_User_LastLoginDate");

            builder.HasIndex(u => u.IsDeleted)
                .HasDatabaseName("IX_User_IsDeleted");
        }
    }

}

