using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Sys
{
    internal class OperationLogEntityConfig:BaseEntityConfig<OperationLogEntity>
    {
        public override void Configure(EntityTypeBuilder<OperationLogEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("OperationLog");

            // 配置操作时间属性
            builder.Property(ol => ol.OperationTime)
                .IsRequired();

            // 配置操作类型属性
            builder.Property(ol => ol.OperationType)
                .IsRequired()
                .HasMaxLength(50);

            // 配置模块属性
            builder.Property(ol => ol.Module)
                .IsRequired()
                .HasMaxLength(100);

            // 配置目标属性
            builder.Property(ol => ol.Target)
                .IsRequired()
                .HasMaxLength(200);

            // 配置详情属性
            builder.Property(ol => ol.Details)
                .IsRequired()
                .HasMaxLength(1000);

            // 配置IP地址属性
            builder.Property(ol => ol.IPAddress)
                .IsRequired()
                .HasMaxLength(50);

            // 配置状态属性
            builder.Property(ol => ol.Status)
                .IsRequired();

            // 配置执行时间属性
            builder.Property(ol => ol.ExecutionTime)
                .IsRequired();

            // 配置外键关系 - 单向导航：OperationLog -> User
            builder.HasOne(ol => ol.User)
                .WithMany() // 单向导航，不在User中配置导航属性
                .HasForeignKey(ol => ol.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除所有操作日志

            // 配置索引
            builder.HasIndex(ol => ol.UserId)
                .HasDatabaseName("IX_OperationLog_UserId");

            builder.HasIndex(ol => ol.OperationTime)
                .HasDatabaseName("IX_OperationLog_OperationTime");

            builder.HasIndex(ol => ol.OperationType)
                .HasDatabaseName("IX_OperationLog_OperationType");

            builder.HasIndex(ol => ol.Module)
                .HasDatabaseName("IX_OperationLog_Module");
        }
    }
}
