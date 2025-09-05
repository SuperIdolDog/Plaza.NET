using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plaza.Net.Model.Entities.Basic;

namespace Plaza.Net.Model.FluentAPIConfigs.Basic
{
    internal class StoreTypeEntityConfig : BaseEntityConfig<StoreTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<StoreTypeEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("StoreType");
            // 配置类型名称属性
            builder.Property(st => st.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
