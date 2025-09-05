using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class StoreEntityConfig : BaseEntityConfig<StoreEntity>
    {
        public override void Configure(EntityTypeBuilder<StoreEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Store");
            // 配置店铺名称属性
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置图片URL属性
            builder.Property(s => s.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.SwiperImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // 配置位置属性
            builder.Property(s => s.Location)
                .IsRequired()
                .HasMaxLength(200);

            // 配置营业时间属性
            builder.Property(s => s.BusinessHours)
                .IsRequired()
                .HasMaxLength(100);

            // 配置联系方式属性
            builder.Property(s => s.Contact)
                .IsRequired()
                .HasMaxLength(100);

            // 配置描述属性
            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // 配置外键关系 - 单向导航：Store -> Floor
            builder.HasOne(s => s.Floor)
                .WithMany() // 单向导航，不在Floor中配置导航属性
                .HasForeignKey(s => s.FloorId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除楼层时删除其所有店铺

            // 配置店铺类型外键
            builder.HasOne(s => s.StoreType)
                .WithMany(s=>s.Stores)
                .HasForeignKey(s => s.StoreTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置店主外键
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(s => s.FloorId)
                .HasDatabaseName("IX_Store_FloorId");

            builder.HasIndex(s => s.StoreTypeId)
                .HasDatabaseName("IX_Store_StoreTypeId");

            builder.HasIndex(s => s.UserId)
                .HasDatabaseName("IX_Store_UserId");
        }
    }
}
