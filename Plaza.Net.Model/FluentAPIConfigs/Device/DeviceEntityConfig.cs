using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.FluentAPIConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Device
{
    internal class DeviceEntityConfig : BaseEntityConfig<DeviceEntity>
    {
        public override void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Device");
            // 配置设备名称属性
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置设备位置属性
            builder.Property(d => d.Location)
                .IsRequired()
                .HasMaxLength(200);

            // 配置外键关系 - 单向导航：Device -> Floor
            builder.HasOne(d => d.Floor)
                .WithMany() // 单向导航，不在Floor中配置导航属性
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除楼层时删除其所有设备

            // 配置设备类型外键
            builder.HasOne(d => d.DeviceType)
                .WithMany(d=>d.Devices)
                .HasForeignKey(d => d.DeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置设备状态外键
            builder.HasOne(d => d.DeviceStatusItem)
                .WithMany()
                .HasForeignKey(d => d.DeviceStatusItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(d => d.FloorId)
                .HasDatabaseName("IX_Device_FloorId");

            builder.HasIndex(d => d.DeviceTypeId)
                .HasDatabaseName("IX_Device_DeviceTypeId");

            builder.HasIndex(d => d.DeviceStatusItemId)
                .HasDatabaseName("IX_Device_DeviceStatusItemId");


        }
    }
}
