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
    internal class ProductSpecValueEntityConfig : BaseEntityConfig<ProductSpecValueEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductSpecValueEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductSpecValue");
            // 配置规格值属性
            builder.Property(psv => psv.Value)
                .IsRequired()
                .HasMaxLength(50);

            // 配置外键关系 - 单向导航：ProductSpecValue -> ProductSpec
            builder.HasOne(psv => psv.Spec)
                .WithMany() // 单向导航，不在ProductSpec中配置导航属性
                .HasForeignKey(psv => psv.SpecId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除规格时删除所有规格值

            // 配置索引
            builder.HasIndex(psv => psv.SpecId)
                .HasDatabaseName("IX_ProductSpecValue_SpecId");

            builder.HasIndex(psv => psv.Value)
                .HasDatabaseName("IX_ProductSpecValue_Value");


        }
    }
}
