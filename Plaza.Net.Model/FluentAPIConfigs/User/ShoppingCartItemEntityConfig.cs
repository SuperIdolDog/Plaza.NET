using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.User
{
    internal class ShoppingCartItemEntityConfig : BaseEntityConfig<ShoppingCartItemEntity>
    {
        public  override void Configure(EntityTypeBuilder<ShoppingCartItemEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("ShoppingCartItem");

            // 配置数量属性
            builder.Property(sci => sci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            // 配置外键关系 - 单向导航：ShoppingCartItem -> User
            builder.HasOne(sci => sci.User)
                .WithMany() // 单向导航，不在User中配置导航属性
                .HasForeignKey(sci => sci.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除购物车项

            // 配置外键关系 - 单向导航：ShoppingCartItem -> Product
            builder.HasOne(sci => sci.ProductSku)
                .WithMany() // 单向导航，不在Product中配置导航属性
                .HasForeignKey(sci => sci.ProductSkuId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除商品时删除购物车项

            // 配置索引
            builder.HasIndex(sci => new { sci.UserId, sci.ProductSkuId })
                .IsUnique()
                .HasDatabaseName("IX_ShoppingCartItem_User_Product");

            builder.HasIndex(sci => sci.UserId)
                .HasDatabaseName("IX_ShoppingCartItem_UserId");

            builder.HasIndex(sci => sci.Selected)
                .HasDatabaseName("IX_ShoppingCartItem_Selected");
        }
    }
}
