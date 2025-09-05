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
    internal class ProductSkuSpecValueEntityConfig : BaseEntityConfig<ProductSkuSpecValueEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductSkuSpecValueEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductSkuSpecValue");

            // 配置外键关系 - 单向导航：ProductSkuSpecValue -> ProductSku
            builder.HasOne(pssv => pssv.ProductSku)
                .WithMany(pssv=>pssv.SpecValueMappings) // 单向导航，不在ProductSku中配置导航属性
                .HasForeignKey(pssv => pssv.ProductSkuId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除SKU时删除关联关系

            // 配置外键关系 - 单向导航：ProductSkuSpecValue -> ProductSpecValue
            builder.HasOne(pssv => pssv.ProductSpecValue)
                .WithMany() // 单向导航，不在ProductSpecValue中配置导航属性
                .HasForeignKey(pssv => pssv.ProductSpecValueId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除规格值时删除关联关系

            // 配置复合唯一索引
            builder.HasIndex(pssv => new { pssv.ProductSkuId, pssv.ProductSpecValueId })
                .IsUnique()
                .HasDatabaseName("IX_ProductSkuSpecValue_Sku_SpecValue");

            // 配置单独索引
            builder.HasIndex(pssv => pssv.ProductSkuId)
                .HasDatabaseName("IX_ProductSkuSpecValue_ProductSkuId");

            builder.HasIndex(pssv => pssv.ProductSpecValueId)
                .HasDatabaseName("IX_ProductSkuSpecValue_ProductSpecValueId");
        }
    }
}
