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
    internal class EmployeeEntityConfig:BaseEntityConfig<EmployeeEntity>
    {
        public override void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Employee");
            // 配置员工姓名属性
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置联系方式属性
            builder.Property(e => e.Contact)
                .IsRequired()
                .HasMaxLength(100);

            // 配置外键关系 - 单向导航：Employee -> Store
            builder.HasOne(e => e.Store)
                .WithMany() // 单向导航，不在Store中配置导航属性
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除店铺时删除所有员工

            // 配置员工角色外键
            builder.HasOne(e => e.EmployeeRole)
                  .WithMany(er => er.Employees) // 双向导航，引用EmployeeRole中的Employees导航属性
                .HasForeignKey(e => e.EmployeeRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(e => e.StoreId)
                .HasDatabaseName("IX_Employee_StoreId");

            builder.HasIndex(e => e.EmployeeRoleId)
                .HasDatabaseName("IX_Employee_EmployeeRoleId");
        }
    }
}
