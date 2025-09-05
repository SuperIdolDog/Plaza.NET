using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Sys
{
   internal class DictionaryItemEntityConfig : BaseEntityConfig<DictionaryItemEntity>
    {
        public override void Configure(EntityTypeBuilder<DictionaryItemEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("DictionaryItem");
            // 配置字典项标签属性
            builder.Property(di => di.Label)
                .IsRequired()
                .HasMaxLength(100);

            // 配置字典项值属性
            builder.Property(di => di.Value)
                .IsRequired()
                .HasMaxLength(100);

            // 配置字典项描述属性
            builder.Property(di => di.Description)
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：DictionaryItem -> Dictionary
            builder.HasOne(di => di.Dictionary)
               .WithMany(d => d.DictionaryItems) // 单向导航，不在Dictionary中配置导航属性
                .HasForeignKey(di => di.DictionaryId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除字典类型时删除所有字典项

            // 配置索引
            builder.HasIndex(di => di.DictionaryId)
                .HasDatabaseName("IX_DictionaryItem_DictionaryId");

            builder.HasIndex(di => di.Value)
                .HasDatabaseName("IX_DictionaryItem_Value");
        }
    }
}
