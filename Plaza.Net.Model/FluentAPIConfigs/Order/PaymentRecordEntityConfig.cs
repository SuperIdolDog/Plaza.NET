using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Order
{
    internal class PaymentRecordEntityConfig : BaseEntityConfig<PaymentRecordEntity>
    {
        public override void Configure(EntityTypeBuilder<PaymentRecordEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("PaymentRecord");

            // 基本属性配置
            builder.Property(p => p.PaymentMethodItemId).IsRequired();
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.PaymentTime).IsRequired();
            builder.Property(p => p.PaystatuItemId).IsRequired();
            builder.Property(p => p.TransactionId).HasMaxLength(100).IsRequired();
            builder.HasOne(pr => pr.Order)
                  .WithMany()
                  .HasForeignKey(pr => pr.OrderId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.User)
                  .WithMany()
                  .HasForeignKey(pr => pr.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
          builder
       .HasOne(p => p.PaymentMethodItem)
       .WithMany()
       .HasForeignKey(p => p.PaymentMethodItemId)
       .OnDelete(DeleteBehavior.Cascade); // 保留级联删除

            builder
                 .HasOne(p => p.PaystatuItem)
                .WithMany()
                .HasForeignKey(p => p.PaystatuItemId)
                .OnDelete(DeleteBehavior.Restrict); // 改为 NO ACTION
        }
    }
}
