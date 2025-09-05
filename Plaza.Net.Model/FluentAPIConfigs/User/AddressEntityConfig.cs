using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FluentAPIConfigs.User
{
    public class AddressEntityConfig : BaseEntityConfig<AddressEntity>
    {
        public override void Configure(EntityTypeBuilder<AddressEntity> builder)
        {
            base.Configure(builder);


            builder.ToTable("Address");

            // 配置收货人姓名属性
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(50);

            // 配置手机号属性
            builder.Property(a => a.Phone)
                .IsRequired()
                .HasMaxLength(20);

            // 配置省市区属性
            builder.Property(a => a.Province)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.County)
                .IsRequired()
                .HasMaxLength(50);

            // 配置详细地址属性
            builder.Property(a => a.Detail)
                .IsRequired()
                .HasMaxLength(200);

            // 配置标签属性
            builder.Property(a => a.Label)
                .HasMaxLength(20);

            // 配置外键关系 - 单向导航：Address -> User
            builder.HasOne(a => a.User)
                .WithMany(a => a.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除所有地址

            // 配置索引
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_Address_UserId");

            builder.HasIndex(a => a.IsDefault)
                .HasDatabaseName("IX_Address_IsDefault");
        }
    }
}
