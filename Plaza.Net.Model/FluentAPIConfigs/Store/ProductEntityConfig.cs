using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Store
{
    internal class ProductEntityConfig:BaseEntityConfig<ProductEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product");
            // 配置商品名称属性
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // 配置商品主图URL属性
            builder.Property(p => p.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // 配置商品描述属性
            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            // 配置外键关系 - 单向导航：Product -> Store
            builder.HasOne(p => p.Store)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除店铺时删除所有商品

            // 配置商品类型外键
            builder.HasOne(p => p.ProductType)
                .WithMany(p=>p.Products)
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(p => p.StoreId)
                .HasDatabaseName("IX_Product_StoreId");

            builder.HasIndex(p => p.ProductTypeId)
                .HasDatabaseName("IX_Product_ProductTypeId");

        }
    }
}
