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
    internal class DictionaryEntityConfig:BaseEntityConfig<DictionaryEntity>
    {
        public override void Configure(EntityTypeBuilder<DictionaryEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Dictionary");
            // 配置字典类型名称属性
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置字典类型描述属性
            builder.Property(d => d.Description)
                .HasMaxLength(500);

            // 配置字典项集合 - 单向导航
            builder.HasMany(d => d.DictionaryItems)
                .WithOne(di => di.Dictionary)
                .HasForeignKey(di => di.DictionaryId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除字典类型时删除所有字典项

        }
    }
}
