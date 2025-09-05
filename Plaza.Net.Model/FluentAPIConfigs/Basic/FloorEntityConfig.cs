using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using System.Reflection.Emit;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class FloorEntityConfig : BaseEntityConfig<FloorEntity>
    {
        public override void Configure(EntityTypeBuilder<FloorEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Floor");
            // 配置楼层名称属性
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 配置楼层描述属性
            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：Floor -> Plaza
            builder.HasOne(f => f.Plaza)
                .WithMany() // 单向导航，不在Plaza中配置导航属性
                .HasForeignKey(f => f.PlazaId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除广场时删除其所有楼层

            // 配置楼层类型外键
            builder.HasOne(f => f.FloorItem)
                .WithMany()
                .HasForeignKey(f => f.FloorItemId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：删除字典项时如果有关联楼层则禁止删除

            // 配置索引
            builder.HasIndex(f => f.PlazaId)
                .HasDatabaseName("IX_Floor_PlazaId");

            builder.HasIndex(f => f.FloorItemId)
                .HasDatabaseName("IX_Floor_FloorItemId");
        }
    }
}
