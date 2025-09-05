using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class PlazaEntityConfig : BaseEntityConfig<PlazaEntity>
    {
        public override void Configure(EntityTypeBuilder<PlazaEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Plaza");
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(200)
                .IsRequired();




            // 多对多关系配置
            builder.HasMany(p => p.Floors)
             .WithOne(f => f.Plaza)
             .HasForeignKey(f => f.PlazaId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
