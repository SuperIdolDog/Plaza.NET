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
    internal class ProductSpecEntityConfig : BaseEntityConfig<ProductSpecEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductSpecEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductSpec");

            // 配置规格名称属性
            builder.Property(ps => ps.Name)
                .IsRequired()
                .HasMaxLength(50);

            // 配置外键关系 - 单向导航：ProductSpec -> Product
            builder.HasOne(ps => ps.Products)
                .WithMany() // 单向导航，不在Product中配置导航属性
                .HasForeignKey(ps => ps.GoodsId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除商品时删除所有规格

            // 配置索引
            builder.HasIndex(ps => ps.GoodsId)
                .HasDatabaseName("IX_ProductSpec_ProductId");

            builder.HasIndex(ps => ps.Sort)
                .HasDatabaseName("IX_ProductSpec_Sort");

        }
    }
}
