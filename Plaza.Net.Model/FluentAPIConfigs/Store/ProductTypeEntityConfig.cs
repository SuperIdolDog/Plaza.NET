using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Store
{
    internal class ProductTypeEntityConfig:BaseEntityConfig<ProductTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductTypeEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductType");
            // 配置类型名称属性
            builder.Property(pt => pt.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置外键关系 - 单向导航：ProductType -> Store
            builder.HasOne(pt => pt.Store)
                .WithMany() // 单向导航，不在Store中配置导航属性
                .HasForeignKey(pt => pt.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除店铺时删除所有商品类型

            // 配置商品单位外键
            builder.HasOne(pt => pt.ProductTypeUnitItem)
                .WithMany()
                .HasForeignKey(pt => pt.ProductTypeUnitItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(pt => pt.StoreId)
                .HasDatabaseName("IX_ProductType_StoreId");

            builder.HasIndex(pt => pt.ProductTypeUnitItemId)
                .HasDatabaseName("IX_ProductType_ProductTypeUnitItemId");
        }
    }
}
