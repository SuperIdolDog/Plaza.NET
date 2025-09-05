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
    internal class LoginLogEntityConfig:BaseEntityConfig<LoginLogEntity>
    {
        public override void Configure(EntityTypeBuilder<LoginLogEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("LoginLog");

            // 配置IP地址属性
            builder.Property(ll => ll.IPAddress)
                .IsRequired()
                .HasMaxLength(50);

            // 配置用户代理属性
            builder.Property(ll => ll.DeviceInfo)
                .HasMaxLength(500);

            // 配置外键关系 - 单向导航：LoginLog -> User
            builder.HasOne(ll => ll.User)
                .WithMany() // 单向导航，不在User中配置导航属性
                .HasForeignKey(ll => ll.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除所有登录日志

            // 配置索引
            builder.HasIndex(ll => ll.UserId)
                .HasDatabaseName("IX_LoginLog_UserId");

            builder.HasIndex(ll => ll.LoginTime)
                .HasDatabaseName("IX_LoginLog_LoginTime");

            builder.HasIndex(ll => ll.IPAddress)
                .HasDatabaseName("IX_LoginLog_IPAddress");

        }
    }
}
