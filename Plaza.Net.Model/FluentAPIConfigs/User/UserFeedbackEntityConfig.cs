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
    internal class UserFeedbackEntityConfig:BaseEntityConfig<UserFeedbackEntity>
    {
        public override void Configure(EntityTypeBuilder<UserFeedbackEntity> builder)
        {
            base.Configure(builder);
            // 配置表名
            builder.ToTable("UserFeedback");

            // 配置反馈图片属性
            builder.Property(uf => uf.Imageurl)
                .IsRequired()
                .HasMaxLength(500);

            // 配置反馈内容属性
            builder.Property(uf => uf.Content)
                .IsRequired()
                .HasMaxLength(1000);

            // 配置联系方式属性
            builder.Property(uf => uf.Contact)
                .IsRequired(false)
                .HasMaxLength(100);

            // 配置回复内容属性
            builder.Property(uf => uf.ReplyContent)
                .IsRequired(false)
                .HasMaxLength(1000);

            // 配置回复者ID属性为可空
            builder.Property(uf => uf.RepliedById)
                .IsRequired(false);

            // 配置外键关系 - 单向导航：UserFeedback -> User (反馈用户)
            builder.HasOne(uf => uf.User)
                .WithMany() // 单向导航，不在User中配置导航属性
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除反馈

            // 配置外键关系 - 单向导航：UserFeedback -> User (回复者)
            builder.HasOne(uf => uf.RepliedBy)
                .WithMany() // 单向导航，不在User中配置导航属性
                .HasForeignKey(uf => uf.RepliedById)
                .OnDelete(DeleteBehavior.SetNull); // 设置为null：删除回复者时设置回复者为null

            // 配置索引
            builder.HasIndex(uf => uf.UserId)
                .HasDatabaseName("IX_UserFeedback_UserId");

            builder.HasIndex(uf => uf.RepliedById)
                .HasDatabaseName("IX_UserFeedback_RepliedById");

            builder.HasIndex(uf => uf.CreateTime)
                .HasDatabaseName("IX_UserFeedback_CreateTime");

            builder.HasIndex(uf => uf.ReplyTime)
                .HasDatabaseName("IX_UserFeedback_ReplyTime");
        }
    }
}
