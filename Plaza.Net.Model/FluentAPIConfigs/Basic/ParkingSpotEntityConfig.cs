using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class ParkingSpotEntityConfig : BaseEntityConfig<ParkingSpotEntity>
    {
        public override void Configure(EntityTypeBuilder<ParkingSpotEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ParkingSpot");
            // 配置停车位名称属性
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            // ParkingArea关系已在ParkingAreaEntityConfig中配置双向关系
            // 配置停车位状态外键
            builder.HasOne(p => p.ParkingSpotItem)
                .WithMany()
                .HasForeignKey(p => p.ParkingSpotItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(p => p.ParkingAreaId)
                .HasDatabaseName("IX_ParkingSpot_ParkingAreaId");

            builder.HasIndex(p => p.ParkingSpotItemId)
                .HasDatabaseName("IX_ParkingSpot_ParkingSpotItemId");


        }
    }
}
