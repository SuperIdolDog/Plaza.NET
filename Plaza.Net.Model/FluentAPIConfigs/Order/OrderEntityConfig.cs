using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Order
{
    internal class OrderEntityConfig:BaseEntityConfig<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            base.Configure(builder); // 调用基类配置

            builder.ToTable("Order");

            // 配置总金额属性
            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            // 配置配送类型属性
            builder.Property(o => o.DeliveryType)
                .IsRequired();

            // 配置配送地址属性
            builder.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：Order -> Store
            builder.HasOne(o => o.Store)
                .WithMany() // 单向导航，不在Store中配置导航属性
                .HasForeignKey(o => o.StoreId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：删除店铺时如果有关联订单则禁止删除

            // 配置客户外键
            builder.HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置员工外键（可选）
            builder.HasOne(o => o.Employee)
                .WithMany(o=>o.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // 设置为空：删除员工时订单保留但员工ID设为空

            // 配置订单状态外键
            builder.HasOne(o => o.OrderStatuItem)
                .WithMany()
                .HasForeignKey(o => o.OrderStatuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            builder.HasIndex(o => o.StoreId)
                .HasDatabaseName("IX_Order_StoreId");

            builder.HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Order_CustomerId");

            builder.HasIndex(o => o.EmployeeId)
                .HasDatabaseName("IX_Order_EmployeeId");

            builder.HasIndex(o => o.OrderStatuItemId)
                .HasDatabaseName("IX_Order_OrderStatuItemId");
        }
    }
}
