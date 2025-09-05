using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Device
{
    internal class ParkingRecordEntityConfig:BaseEntityConfig<ParkingRecordEntity>
    {
        public override void Configure(EntityTypeBuilder<ParkingRecordEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ParkingRecord");
            // 配置车牌号码属性
            builder.Property(pr => pr.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            // 配置车牌识别图片属性
            builder.Property(pr => pr.PlateImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：ParkingRecord -> Device
            builder.HasOne(pr => pr.Device)
                .WithMany() // 单向导航，不在Device中配置导航属性
                .HasForeignKey(pr => pr.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除设备时删除所有停车记录

            // 配置索引
            builder.HasIndex(pr => pr.DeviceId)
                .HasDatabaseName("IX_ParkingRecord_DeviceId");

            builder.HasIndex(pr => pr.PlateNumber)
                .HasDatabaseName("IX_ParkingRecord_PlateNumber");

            builder.HasIndex(pr => pr.EntryTime)
                .HasDatabaseName("IX_ParkingRecord_EntryTime");
        }
    }
}
