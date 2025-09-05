using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plaza.Net.Model.Entities.User;

namespace Plaza.Net.Model.FluentAPIConfigs.User
{
    internal class NotificationEntityConfig : BaseEntityConfig<NotificationEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Notification");
            // 配置标题属性
            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(100);

            // 配置内容属性
            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(1000);

            // 配置链接属性
            builder.Property(n => n.Link)
                .IsRequired()
                .HasMaxLength(500);

            // 配置外键关系：Notification -> User (接收者)
            builder.HasOne(n => n.User)
                .WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 级联删除：删除用户时删除通知

            // 配置外键关系：Notification -> User (发布者)
            builder.HasOne(n => n.Publisher)
                .WithMany(u => u.PublishedNotifications)
                .HasForeignKey(n => n.PublisherId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：有通知时不能删除发布者

            // 配置外键关系 - 单向导航：Notification -> DictionaryItem (通知类型)
            builder.HasOne(n => n.NotificationTypeItem)
                .WithMany() // 单向导航，不在DictionaryItem中配置导航属性
                .HasForeignKey(n => n.NotificationTypeItemId)
                .OnDelete(DeleteBehavior.Restrict); // 限制删除：有通知时不能删除字典项

            // 配置索引
            builder.HasIndex(n => n.UserId)
                .HasDatabaseName("IX_Notification_UserId");

            builder.HasIndex(n => n.PublisherId)
                .HasDatabaseName("IX_Notification_PublisherId");

            builder.HasIndex(n => n.NotificationTypeItemId)
                .HasDatabaseName("IX_Notification_NotificationTypeItemId");

            builder.HasIndex(n => n.IsRead)
                .HasDatabaseName("IX_Notification_IsRead");

            builder.HasIndex(n => n.CreateTime)
                .HasDatabaseName("IX_Notification_CreateTime");
        }
    }
}
