using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Store
{
    internal class EmployeeRoleEntityConfig:BaseEntityConfig<EmployeeRoleEntity>
    {
        public override void Configure(EntityTypeBuilder<EmployeeRoleEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("EmployeeRole");
            // 配置角色名称属性
            builder.Property(er => er.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置角色描述属性
            builder.Property(er => er.Description)
                .IsRequired()
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：EmployeeRole -> Store
            builder.HasOne(er => er.Store)
                .WithMany() // 单向导航，不在Store中配置导航属性
                .HasForeignKey(er => er.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除店铺时删除所有员工角色

            // 配置索引
            builder.HasIndex(er => er.StoreId)
                .HasDatabaseName("IX_EmployeeRole_StoreId");
        }
    }
}
