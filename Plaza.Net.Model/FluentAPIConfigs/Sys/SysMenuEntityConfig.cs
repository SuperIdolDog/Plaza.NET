using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Sys
{
    internal class SysMenuEntityConfig :BaseEntityConfig<SysMenuEntity>
    {
        public override void Configure(EntityTypeBuilder<SysMenuEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("SysMenu");
            // 配置菜单名称属性
            builder.Property(sm => sm.Name)
                .IsRequired()
                .HasMaxLength(50);

            // 配置菜单图标属性
            builder.Property(sm => sm.Icon)
                .HasMaxLength(50);

            // 配置菜单URL属性
            builder.Property(sm => sm.Url)
                .HasMaxLength(200);

            // 配置菜单类型属性
            builder.Property(sm => sm.Type)
                .IsRequired()
                .HasMaxLength(20);

            // 配置自引用关系 - 父菜单
            builder.HasOne(sm => sm.Parent)
                .WithMany(sm => sm.Children)
                .HasForeignKey(sm => sm.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：有子菜单时不能删除父菜单

            // 配置索引
            builder.HasIndex(sm => sm.ParentId)
                .HasDatabaseName("IX_SysMenu_ParentId");

            builder.HasIndex(sm => sm.Order)
                .HasDatabaseName("IX_SysMenu_Order");

            builder.HasIndex(sm => sm.Type)
                .HasDatabaseName("IX_SysMenu_Type");


        }
    }


}


