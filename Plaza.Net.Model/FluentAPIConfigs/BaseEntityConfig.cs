using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs
{
    public class BaseEntityConfig<T>:IEntityTypeConfiguration<T> where T : BaseEntity
    {


        public virtual  void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            //builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Code).HasMaxLength(50);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP(6)") // MySQL 函数
              .ValueGeneratedOnAdd();
            // .HasDefaultValueSql("CURRENT_TIMESTAMP(6)") ;// MySQL 兼容的当前时间函数 //.HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.UpdateTime).IsRequired(false);


        }
    }
}

