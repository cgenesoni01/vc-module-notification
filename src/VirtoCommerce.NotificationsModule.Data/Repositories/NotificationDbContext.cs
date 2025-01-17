using System.Reflection;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationDbContext : DbContextWithTriggers
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        protected NotificationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Notification

            modelBuilder.Entity<NotificationEntity>().ToTable("Notification").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<NotificationEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<NotificationEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationEntity>().Property("Discriminator").HasMaxLength(128);
            modelBuilder.Entity<NotificationEntity>().HasIndex(x => new { x.Type, x.TenantId, x.TenantType });

            modelBuilder.Entity<EmailNotificationEntity>();
            modelBuilder.Entity<SmsNotificationEntity>();

            #endregion

            #region NotificationTemplate

            modelBuilder.Entity<NotificationTemplateEntity>().ToTable("NotificationTemplate").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationTemplateEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationTemplateEntity>().Property("Discriminator").HasMaxLength(128);

            modelBuilder.Entity<NotificationTemplateEntity>().HasOne(x => x.Notification).WithMany(x => x.Templates)
                       .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailNotificationTemplateEntity>();
            modelBuilder.Entity<EmailNotificationTemplateEntity>().HasOne(x => x.NotificationLayout)
                        .WithMany()
                        .HasForeignKey(x => x.NotificationLayoutId)
                        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SmsNotificationTemplateEntity>();

            #endregion

            #region NotificationMessage

            modelBuilder.Entity<NotificationMessageEntity>().ToTable("NotificationMessage").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationMessageEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationMessageEntity>().Property("Discriminator").HasMaxLength(128);

            modelBuilder.Entity<NotificationMessageEntity>().HasOne(x => x.Notification).WithMany()
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailNotificationMessageEntity>();
            modelBuilder.Entity<SmsNotificationMessageEntity>();

            #endregion

            #region EmailAttachment

            modelBuilder.Entity<EmailAttachmentEntity>().ToTable("NotificationEmailAttachment").HasKey(x => x.Id);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            modelBuilder.Entity<EmailAttachmentEntity>().HasOne(x => x.Notification).WithMany(x => x.Attachments)
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region NotificationEmailRecipient

            modelBuilder.Entity<NotificationEmailRecipientEntity>().ToTable("NotificationEmailRecipient").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEmailRecipientEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();

            modelBuilder.Entity<NotificationEmailRecipientEntity>().HasOne(x => x.Notification).WithMany(x => x.Recipients)
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region NotificationLayout

            modelBuilder.Entity<NotificationLayoutEntity>().ToTable("NotificationLayout").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationLayoutEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<NotificationLayoutEntity>().HasIndex(x => x.Name).IsUnique();

            #endregion

            base.OnModelCreating(modelBuilder);

            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.NotificationsModule.Data.XXX project. />
            switch (this.Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.NotificationsModule.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.NotificationsModule.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.NotificationsModule.Data.SqlServer"));
                    break;
            }
        }
    }
}
