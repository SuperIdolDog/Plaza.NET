using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Device;


namespace Plaza.Net.Model.FluentAPIConfigs.Device
{
    internal class DeviceDataEntityConfig:BaseEntityConfig<DeviceDataEntity>
    {
        public override void Configure(EntityTypeBuilder<DeviceDataEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("DeviceData");

            // 配置外键关系 - 单向导航：DeviceData -> Device
            builder.HasOne(dd => dd.Device)
                .WithMany() // 单向导航，不在Device中配置导航属性
                .HasForeignKey(dd => dd.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除设备时删除所有设备数据

            // 配置数据单位外键
            builder.HasOne(dd => dd.DeviceDataUnitItem)
                .WithMany()
                .HasForeignKey(dd => dd.DeviceDataUnitItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(dd => dd.DeviceId)
                .HasDatabaseName("IX_DeviceData_DeviceId");

            builder.HasIndex(dd => dd.DeviceDataUnitItemId)
                .HasDatabaseName("IX_DeviceData_DeviceDataUnitItemId");

        }
    }
}
