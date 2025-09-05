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
    internal class ProductSkuEntityConfig : BaseEntityConfig<ProductSkuEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductSkuEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductSku");

            // 配置SKU名称属性
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // 配置SKU编码属性
            builder.Property(p => p.SkuCode)
                .IsRequired()
                .HasMaxLength(100);

            // 配置条形码属性
            builder.Property(p => p.BarCode)
                .HasMaxLength(100);

            // 配置SKU主图属性
            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            // 配置价格相关属性
            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Property(p => p.CostPrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.MarketPrice)
                .HasPrecision(18, 2);

            // 配置库存和销量
            builder.Property(p => p.StockQuantity)
                .HasPrecision(18, 2);

            builder.Property(p => p.Weight)
                .HasPrecision(18, 2);

            // 配置外键关系 - 单向导航：ProductSku -> Product
            builder.HasOne(p => p.Product)
                .WithMany(p=>p.Skus) // 单向导航，不在Product中配置导航属性
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除商品时删除所有SKU

            // 配置索引
            builder.HasIndex(p => p.ProductId)
                .HasDatabaseName("IX_ProductSku_ProductId");

            builder.HasIndex(p => p.SkuCode)
                .IsUnique()
                .HasDatabaseName("IX_ProductSku_SkuCode");

            builder.HasIndex(p => p.BarCode)
                .HasDatabaseName("IX_ProductSku_BarCode");
        }
    }

}
