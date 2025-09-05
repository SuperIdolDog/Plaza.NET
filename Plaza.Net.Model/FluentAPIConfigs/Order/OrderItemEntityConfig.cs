using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Order
{
    internal class OrderItemEntityConfig : BaseEntityConfig<OrderItemEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("OrderItem");

            // 配置外键关系 - 单向导航：OrderItem -> Order
            builder.HasOne(oi => oi.Order)
               .WithMany(o => o.Items) // 双向导航，配置Order中的Items集合
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除订单时删除所有订单项

            // 配置商品外键
            builder.HasOne(oi => oi.ProductSku)
                .WithMany()
                .HasForeignKey(oi => oi.ProductSkuId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_OrderItem_OrderId");

            builder.HasIndex(oi => oi.ProductSkuId)
                .HasDatabaseName("IX_OrderItem_ProductId");
        }
    }
}
