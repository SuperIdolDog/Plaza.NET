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
    internal class ReviewEntityConfig : BaseEntityConfig<ReviewEntity>
    {
        public override void Configure(EntityTypeBuilder<ReviewEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Review");

            // 基本属性配置
            builder.Property(r => r.ReviewRatingItemId).IsRequired();
            builder.Property(r => r.Content).HasMaxLength(1000);

            builder.HasOne(r => r.OrderItem)
                 .WithMany()
                 .HasForeignKey(r => r.OrderItemId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
