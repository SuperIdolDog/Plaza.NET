using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Device
{
    internal class DeviceTypeEntityConfig:BaseEntityConfig<DeviceTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<DeviceTypeEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("DeviceType");
            // 配置类型名称属性
            builder.Property(dt => dt.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置类型描述属性
            builder.Property(dt => dt.Description)
                .IsRequired()
                .HasMaxLength(500);

            // 配置制造商属性
            builder.Property(dt => dt.Manufacturer)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
