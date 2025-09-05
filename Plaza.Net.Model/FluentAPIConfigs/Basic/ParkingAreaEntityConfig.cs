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
    internal class ParkingAreaEntityConfig : BaseEntityConfig<ParkingAreaEntity>
    {
        public override void Configure(EntityTypeBuilder<ParkingAreaEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ParkingArea");
            // 配置区域名称属性
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置外键关系 - 单向导航：ParkingArea -> Floor
            builder.HasOne(p => p.Floor)
                .WithMany(f => f.ParkingAreas)
                .HasForeignKey(p => p.FloorId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除楼层时删除其所有停车区域

            // 配置停车区域类型外键
            builder.HasOne(p => p.ParkingAreaItem)
                .WithMany()
                .HasForeignKey(p => p.ParkingAreaItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置停车位集合导航
            builder.HasMany(p => p.ParkingSpot)
                .WithOne(ps => ps.ParkingArea)
                .HasForeignKey(ps => ps.ParkingAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置索引
            builder.HasIndex(p => p.FloorId)
                .HasDatabaseName("IX_ParkingArea_FloorId");

            builder.HasIndex(p => p.ParkingAreaItemId)
                .HasDatabaseName("IX_ParkingArea_ParkingAreaItemId");
        }
    }
}
