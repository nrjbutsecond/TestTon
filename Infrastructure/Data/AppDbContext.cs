using BCrypt.Net;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Admin;
using Domain.Entities.Mentor;
using Domain.Entities.MerchandiseEntity;
using Domain.Entities.Organize;
using Domain.Entities.ServicePlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Org.BouncyCastle.Crypto.Generators;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ticket.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<ArticleTagModel> ArticleTags { get; set; }
        public DbSet<WorkshopModel> Workshops { get; set; }
        public DbSet<TicketTypeModel> TicketTypes { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }
        public DbSet<TalkEventModel> TalkEvents { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<ShippingConfig> shippingConfigs { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<SponsorModel> Sponsors { get; set; }
        public DbSet<AdvertisementModel> Advertisements { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public DbSet<PartnershipApplication> PartnershipApplications { get; set; }
        public DbSet<OrganizationActivity> OrganizationActivities { get; set; }
        public DbSet<OrganizationStatistics> OrganizationStatistics { get; set; }
        public DbSet<Merchandise> Merchandises { get; set; }
        public DbSet<MerchandiseVariant> MerchandiseVariants { get; set; }
        // Analytics Entities
        public DbSet<KpiSnapshot> KpiSnapshots { get; set; }
        public DbSet<RevenueAnalytics> RevenueAnalytics { get; set; }
        public DbSet<UserAnalytics> UserAnalytics { get; set; }
        public DbSet<ServiceAnalytics> ServiceAnalytics { get; set; }
        public DbSet<OrganizationPerformance> OrganizationPerformances { get; set; }
        public DbSet<PlatformUsage> PlatformUsages { get; set; }
        public DbSet<GeographicAnalytics> GeographicAnalytics { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<PerformanceAlert> PerformanceAlerts { get; set; }
        public DbSet<SystemStatistics> SystemStatistics { get; set; }
        public DbSet<ConversionFunnel> ConversionFunnels { get; set; }
        public DbSet<RevenueBreakdown> RevenueBreakdowns { get; set; }
        public DbSet<UserEngagementMetrics> UserEngagementMetrics { get; set; }
        public DbSet<OrderAnalytics> OrderAnalytics { get; set; }
        public DbSet<DashboardConfiguration> DashboardConfigurations { get; set; }
        public DbSet<SupportPersonnel> SupportPersonnel { get; set; }
        public DbSet<PersonnelSupportRequest> PersonnelSupportRequests { get; set; }
        public DbSet<PersonnelSupportAssignment> PersonnelSupportAssignments { get; set; }
        public DbSet<ServicePlanModel> ServicePlans { get; set; }
        public DbSet<UserServicePlanSubscriptionModel> UserServicePlanSubscriptions { get; set; }
        public DbSet<ContractNegotiationModel> ContractNegotiations { get; set; }
        public DbSet<ConsultationRequest> ConsultationRequests { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<SupportTicketMessage> SupportTicketMessages { get; set; }

        //Mentor
        public DbSet<MentoringRecord> MentoringRecords { get; set; }
        public DbSet<MentoringSessionParticipant> MentoringSessionParticipants { get; set; }
        public DbSet<MentoringSessionAttachment> MentoringSessionAttachments { get; set; }
        public DbSet<MentorAvailability> MentorAvailabilities { get; set; }
        public DbSet<MentorBlockedTime> MentorBlockedTimes { get; set; }

        //notify
        public DbSet<Notification> Notifications { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure computed properties
            modelBuilder.Entity<AdvertisementModel>().Ignore(a => a.RemainingBudget);
            modelBuilder.Entity<AdvertisementModel>().Ignore(a => a.ClickThroughRate);
            modelBuilder.Entity<AdvertisementModel>().Ignore(a => a.IsExpired);
            modelBuilder.Entity<AdvertisementModel>().Ignore(a => a.IsScheduledToStart);

            // Use IDENTITY for auto-incrementing IDs (suitable for PostgreSQL)
            modelBuilder.UseIdentityColumns();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configure string properties to use text (fine for PostgreSQL)
                var stringProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string));

                foreach (var property in stringProperties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasColumnType("text");
                }

                // Configure decimal properties to use numeric (equivalent to decimal in PG)
                var decimalProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasColumnType("numeric(18,2)");
                }
            }

            // User configurations
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.Phone)
                    .HasFilter("\"Phone\" IS NOT NULL AND \"IsDeleted\" = false");

                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => new { e.IsActive, e.EmailConfirmed });
                entity.HasIndex(e => new { e.ServicePlan, e.ServicePlanExpiry });

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>()
                    .HasDefaultValue(UserRoles.Enthusiast);

                entity.Property(e => e.ServicePlan)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Free");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.EmailConfirmed)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsPartneredOrganizer)
                    .HasDefaultValue(false);

                entity.Property(e => e.EmailConfirmationToken)
                    .HasMaxLength(500);

                entity.Property(e => e.PasswordResetToken)
                    .HasMaxLength(500);

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(500);

                entity.Property(e => e.RefreshTokenExpiry)
                    .IsRequired(false);

                entity.HasIndex(e => e.RefreshToken)
                    .HasFilter("\"RefreshToken\" IS NOT NULL");
            });

            // User relationships
            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.OrganizedEvents)
                .WithOne(e => e.Organizer)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.PurchasedTickets)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Articles)
                .WithOne(a => a.AuthorUser)
                .HasForeignKey(a => a.Author)
                .OnDelete(DeleteBehavior.Restrict);

            // Merchandise configurations
            modelBuilder.Entity<Merchandise>(entity =>
            {
                entity.ToTable("Merchandises");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.SKU)
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => new { e.IsActive, e.IsOfficial });
                entity.HasIndex(e => e.SellerId)
                    .HasFilter("\"SellerId\" IS NOT NULL");

                entity.Property(e => e.SKU)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Images)
                    .HasColumnType("jsonb");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.IsOfficial)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Merchandise>()
                .HasOne(m => m.Seller)
                .WithMany(u => u.Merchandises)
                .HasForeignKey(m => m.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // MerchandiseVariant configurations
            modelBuilder.Entity<MerchandiseVariant>(entity =>
            {
                entity.ToTable("MerchandiseVariants");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.MerchandiseId, e.Name, e.Value })
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MerchandiseVariant>()
                .HasOne(v => v.Merchandise)
                .WithMany(m => m.Variants)
                .HasForeignKey(v => v.MerchandiseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart configurations
            modelBuilder.Entity<CartModel>(entity =>
            {
                entity.ToTable("Carts");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpiresAt);

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();
            });

            modelBuilder.Entity<CartModel>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<CartModel>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem configurations
            modelBuilder.Entity<CartItemModel>(entity =>
            {
                entity.ToTable("CartItems");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.CartId, e.MerchandiseId, e.VariantId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.CartModel)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.Merchandise)
                .WithMany()
                .HasForeignKey(ci => ci.MerchandiseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.Variant)
                .WithMany()
                .HasForeignKey(ci => ci.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Workshop relationships
            modelBuilder.Entity<WorkshopModel>()
                .HasOne(w => w.Organizer)
                .WithMany()
                .HasForeignKey(w => w.OrganizerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Article configurations
            modelBuilder.Entity<ArticleModel>(entity =>
            {
                entity.ToTable("Articles");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.Summary)
                    .HasMaxLength(500);

                entity.Property(e => e.FeaturedImage)
                    .HasMaxLength(500);

                entity.Property(e => e.MetaTitle)
                    .HasMaxLength(60);

                entity.Property(e => e.MetaDescription)
                    .HasMaxLength(160);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasDatabaseName("IX_Articles_Slug");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Articles_Status");

                entity.HasIndex(e => e.PublishedAt)
                    .HasDatabaseName("IX_Articles_PublishedAt");

                entity.HasIndex(e => e.Author)
                    .HasDatabaseName("IX_Articles_Author");

                entity.HasOne(a => a.AuthorUser)
                    .WithMany()
                    .HasForeignKey(a => a.Author)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Articles_Users_Author");
            });

            // ArticleTag configurations
            modelBuilder.Entity<ArticleTagModel>(entity =>
            {
                entity.ToTable("ArticleTags");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasDatabaseName("IX_ArticleTags_Slug");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_ArticleTags_Name");
            });

            // Many-to-Many relationship between Article and ArticleTag
            modelBuilder.Entity<ArticleModel>()
                .HasMany(a => a.Tags)
                .WithMany(t => t.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleArticleTags",
                    j => j.HasOne<ArticleTagModel>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<ArticleModel>()
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("ArticleId", "TagId");
                        j.ToTable("ArticleArticleTags");
                        j.HasIndex("TagId").HasDatabaseName("IX_ArticleArticleTags_TagId");
                        j.HasIndex("ArticleId").HasDatabaseName("IX_ArticleArticleTags_ArticleId");
                    });

            // UserActivityLog configuration
            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.ToTable("UserActivityLogs");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.ActivityType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Path)
                    .HasMaxLength(500);

                entity.Property(e => e.Method)
                    .HasMaxLength(10);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);

                entity.HasIndex(e => new { e.UserId, e.Timestamp })
                    .HasDatabaseName("IX_UserActivityLogs_UserId_Timestamp");

                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName("IX_UserActivityLogs_Timestamp");

                entity.HasIndex(e => e.ActivityType)
                    .HasDatabaseName("IX_UserActivityLogs_ActivityType");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserActivityLogs_Users_UserId");
            });

            // UserSession configuration
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.ToTable("UserSessions");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.StartedAt)
                    .IsRequired();

                entity.Property(e => e.LastPingAt)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);

                entity.Property(e => e.DeviceInfo)
                    .HasMaxLength(200);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);

                entity.HasIndex(e => new { e.UserId, e.IsActive })
                    .HasDatabaseName("IX_UserSessions_UserId_IsActive");

                entity.HasIndex(e => e.LastPingAt)
                    .HasDatabaseName("IX_UserSessions_LastPingAt");

                entity.HasIndex(e => new { e.IsActive, e.LastPingAt })
                    .HasDatabaseName("IX_UserSessions_IsActive_LastPingAt");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserSessions_Users_UserId");
            });

            // ShippingConfig configurations
            modelBuilder.Entity<ShippingConfig>(entity =>
            {
                entity.ToTable("ShippingConfigs");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.City, e.IsActive })
                    .IsUnique()
                    .HasFilter("\"IsActive\" = true AND \"IsDeleted\" = false");

                entity.HasIndex(e => new { e.IsDefault, e.IsActive })
                    .IsUnique()
                    .HasFilter("\"IsDefault\" = true AND \"IsActive\" = true AND \"IsDeleted\" = false");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BaseFee)
                    .HasPrecision(18, 2);

                entity.Property(e => e.FreeShippingThreshold)
                    .HasPrecision(18, 2);

                entity.Property(e => e.BulkOrderThreshold)
                    .HasPrecision(18, 2);

                entity.Property(e => e.BulkOrderExtraFee)
                    .HasPrecision(18, 2);
            });

            // Workshop configurations
            modelBuilder.Entity<WorkshopModel>(entity =>
            {
                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OnlineLink)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue(WorkshopStatus.Draft);

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(500);

                entity.HasOne(w => w.Organizer)
                    .WithMany()
                    .HasForeignKey(w => w.OrganizerId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TalkEvent configurations
            modelBuilder.Entity<TalkEventModel>(entity =>
            {
                entity.ToTable("TalkEvents");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.MaxAttendees)
                    .IsRequired();

                entity.Property(e => e.HasTicketSale)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.BannerImage)
                    .HasMaxLength(500);

                entity.Property(e => e.ThumbnailImage)
                    .HasMaxLength(500);

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(255);

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.DeletedAt);

                entity.HasOne(e => e.Organizer)
                    .WithMany()
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.TicketTypes)
                    .WithOne()
                    .HasForeignKey("TalkEventId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Tickets)
                    .WithOne()
                    .HasForeignKey("TicketableId")
                    .HasPrincipalKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.StartDate)
                    .HasDatabaseName("IX_TalkEvents_StartDate");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_TalkEvents_Status");

                entity.HasIndex(e => e.OrganizerId)
                    .HasDatabaseName("IX_TalkEvents_OrganizerId");

                entity.HasIndex(e => new { e.Status, e.StartDate })
                    .HasDatabaseName("IX_TalkEvents_Status_StartDate");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Review configurations
            modelBuilder.Entity<ReviewModel>(entity =>
            {
                entity.ToTable("Reviews");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.UserId, e.MerchandiseId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.MerchandiseId);
                entity.HasIndex(e => e.Rating);
                entity.HasIndex(e => e.IsVerifiedPurchase);

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasMaxLength(200);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Images)
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Merchandise)
                .WithMany()
                .HasForeignKey(r => r.MerchandiseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.OrderItem)
                .WithMany()
                .HasForeignKey(r => r.OrderItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReviewResponse configurations
            modelBuilder.Entity<ReviewResponseModel>(entity =>
            {
                entity.ToTable("ReviewResponses");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.ReviewId);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<ReviewResponseModel>()
                .HasOne(rr => rr.Review)
                .WithMany(r => r.Responses)
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewResponseModel>()
                .HasOne(rr => rr.User)
                .WithMany()
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReviewVote configurations
            modelBuilder.Entity<ReviewVoteModel>(entity =>
            {
                entity.ToTable("ReviewVotes");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.UserId, e.ReviewId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");
            });

            modelBuilder.Entity<ReviewVoteModel>()
                .HasOne(rv => rv.Review)
                .WithMany(r => r.Votes)
                .HasForeignKey(rv => rv.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewVoteModel>()
                .HasOne(rv => rv.User)
                .WithMany()
                .HasForeignKey(rv => rv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketType configurations
            modelBuilder.Entity<TicketTypeModel>(entity =>
            {
                entity.ToTable("TicketTypes");

                entity.HasKey(tt => tt.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(tt => tt.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(tt => tt.Price)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(tt => tt.MaxQuantity)
                    .IsRequired();

                entity.Property(tt => tt.SoldQuantity)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(tt => tt.Benefits)
                    .IsRequired(false);

                entity.Property(tt => tt.SaleStartDate)
                    .IsRequired();

                entity.Property(tt => tt.SaleEndDate)
                    .IsRequired();

                entity.Property(tt => tt.ReasonDelete)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.HasOne(tt => tt.TalkEvent)
                    .WithMany(te => te.TicketTypes)
                    .HasForeignKey(tt => tt.TalkEventId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(tt => tt.Workshop)
                    .WithMany(w => w.TicketTypes)
                    .HasForeignKey(tt => tt.WorkshopId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(tt => tt.TalkEventId)
                    .HasDatabaseName("IX_TicketTypes_TalkEventId");

                entity.HasIndex(tt => tt.WorkshopId)
                    .HasDatabaseName("IX_TicketTypes_WorkshopId");

                entity.HasIndex(tt => new { tt.SaleStartDate, tt.SaleEndDate })
                    .HasDatabaseName("IX_TicketTypes_SaleDates");

                entity.ToTable("TicketTypes", t =>
                {
                    t.HasCheckConstraint("CK_TicketType_OneEventType",
                        "(\"TalkEventId\" IS NOT NULL AND \"WorkshopId\" IS NULL) OR (\"TalkEventId\" IS NULL AND \"WorkshopId\" IS NOT NULL)");

                    t.HasCheckConstraint("CK_TicketType_SaleDates",
                        "\"SaleEndDate\" > \"SaleStartDate\"");

                    t.HasCheckConstraint("CK_TicketType_Quantities",
                        "\"SoldQuantity\" >= 0 AND \"MaxQuantity\" > 0 AND \"SoldQuantity\" <= \"MaxQuantity\"");

                    t.HasCheckConstraint("CK_TicketType_Price",
                        "\"Price\" >= 0");
                });
            });

            // Ticket configurations
            modelBuilder.Entity<TicketModel>(entity =>
            {
                entity.ToTable("Tickets");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.TicketableId)
                    .IsRequired();

                entity.Property(e => e.TicketableType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>();

                entity.Property(e => e.TicketTypeId)
                    .IsRequired();

                entity.Property(e => e.Guid)
                    .IsRequired()
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.QRCode)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>()
                    .HasDefaultValue(TicketStatus.Reserved);

                entity.Property(e => e.PurchaseDate)
                    .IsRequired(false);

                entity.Property(e => e.ValidFrom)
                    .IsRequired();

                entity.Property(e => e.ValidUntil)
                    .IsRequired();

                entity.Property(e => e.ReasonDelete)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.PurchasedTickets)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.TicketType)
                    .WithMany(tt => tt.Tickets)
                    .HasForeignKey(t => t.TicketTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Guid)
                    .IsUnique()
                    .HasDatabaseName("IX_Tickets_Guid");

                entity.HasIndex(e => e.QRCode)
                    .IsUnique()
                    .HasDatabaseName("IX_Tickets_QRCode");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_Tickets_UserId");

                entity.HasIndex(e => e.TicketTypeId)
                    .HasDatabaseName("IX_Tickets_TicketTypeId");

                entity.HasIndex(e => new { e.TicketableId, e.TicketableType })
                    .HasDatabaseName("IX_Tickets_Ticketable");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Tickets_Status");

                entity.HasIndex(e => new { e.ValidFrom, e.ValidUntil })
                    .HasDatabaseName("IX_Tickets_ValidDates");

                entity.HasIndex(e => new { e.UserId, e.Status, e.IsDeleted })
                    .HasDatabaseName("IX_Tickets_UserId_Status_IsDeleted");

                entity.HasIndex(e => new { e.TicketableId, e.TicketableType, e.Status })
                    .HasDatabaseName("IX_Tickets_Ticketable_Status");

                entity.ToTable("Tickets", t =>
                {
                    t.HasCheckConstraint("CK_Ticket_ValidDates",
                        "\"ValidUntil\" > \"ValidFrom\"");

                    t.HasCheckConstraint("CK_Ticket_TicketableType",
                        "\"TicketableType\" IN ('TalkEvent', 'Workshop')");

                    t.HasCheckConstraint("CK_Ticket_Status",
                        "\"Status\" IN ('Reserved', 'Paid', 'Used', 'Cancelled', 'Expired')");
                });
            });

            // Sponsor configurations
            modelBuilder.Entity<SponsorModel>(entity =>
            {
                entity.ToTable("Sponsors");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(s => s.Logo)
                    .HasMaxLength(500);

                entity.Property(s => s.Website)
                    .HasMaxLength(500);

                entity.Property(s => s.Description)
                    .HasMaxLength(2000);

                entity.Property(s => s.ContactPerson)
                    .HasMaxLength(200);

                entity.Property(s => s.ContactEmail)
                    .HasMaxLength(255);

                entity.Property(s => s.ContactPhone)
                    .HasMaxLength(20);

                entity.Property(s => s.SponsorshipLevel)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue(SponsorshipLevelEnum.Bronze);

                entity.Property(s => s.ContributionAmount)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0);

                entity.Property(s => s.ContractStartDate)
                    .HasColumnType("timestamp with time zone");

                entity.Property(s => s.ContractEndDate)
                    .HasColumnType("timestamp with time zone");

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);

                entity.Property(s => s.Benefits)
                    .HasColumnType("jsonb");

                entity.Property(s => s.DisplayOrder)
                    .HasDefaultValue(0);

                entity.HasIndex(s => s.Name).HasDatabaseName("IX_Sponsors_Name");
                entity.HasIndex(s => s.ContactEmail).HasDatabaseName("IX_Sponsors_ContactEmail");
                entity.HasIndex(s => s.SponsorshipLevel).HasDatabaseName("IX_Sponsors_SponsorshipLevel");
                entity.HasIndex(s => new { s.IsActive, s.IsDeleted }).HasDatabaseName("IX_Sponsors_IsActive_IsDeleted");
                entity.HasIndex(s => s.DisplayOrder).HasDatabaseName("IX_Sponsors_DisplayOrder");

                entity.HasQueryFilter(s => !s.IsDeleted);

                entity.HasCheckConstraint(
                    "CK_Sponsors_SponsorshipLevel",
                    "\"SponsorshipLevel\" IN ('Bronze', 'Silver', 'Gold', 'Platinum')");

                entity.HasCheckConstraint("CK_Sponsors_ContributionAmount", "\"ContributionAmount\" >= 0");

                entity.HasCheckConstraint("CK_Sponsors_ContractDates",
                    "\"ContractEndDate\" IS NULL OR \"ContractStartDate\" IS NULL OR \"ContractEndDate\" >= \"ContractStartDate\"");
            });

            // Order configurations
            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique();

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>();

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShippingAddress)
                    .HasColumnType("jsonb");

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem configurations
            modelBuilder.Entity<OrderItemModel>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.HasKey(e => e.Id);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.OrderId);
            });

            modelBuilder.Entity<OrderItemModel>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItemModel>()
                .HasOne(oi => oi.Merchandise)
                .WithMany()
                .HasForeignKey(oi => oi.MerchandiseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItemModel>()
                .HasOne(oi => oi.Variant)
                .WithMany()
                .HasForeignKey(oi => oi.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization configurations
            modelBuilder.Entity<OrganizationModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Website).HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.Property(e => e.LogoUrl).HasMaxLength(500);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
                entity.Property(e => e.Rating).HasPrecision(3, 1);
                entity.Property(e => e.MonthlyRevenue).HasPrecision(18, 2);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => new { e.Status, e.PartnershipTier });

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // OrganizationMember Entity Configuration
            modelBuilder.Entity<OrganizationMember>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Organization)
                    .WithMany(o => o.Members)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.OrganizationId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // PartnershipApplication Entity Configuration
            modelBuilder.Entity<PartnershipApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ApplicationReason).HasMaxLength(1000);
                entity.Property(e => e.ReviewNotes).HasMaxLength(1000);

                entity.HasOne(e => e.Organization)
                    .WithMany(o => o.Applications)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReviewedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.OrganizationId, e.Status });
                entity.HasIndex(e => e.ApplicationDate);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // OrganizationActivity Entity Configuration
            modelBuilder.Entity<OrganizationActivity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Details).HasMaxLength(2000);

                entity.HasOne(e => e.Organization)
                    .WithMany(o => o.Activities)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.OrganizationId, e.ActivityDate });

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // OrganizationStatistics Entity Configuration
            modelBuilder.Entity<OrganizationStatistics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Revenue).HasPrecision(18, 2);
                entity.Property(e => e.AverageRating).HasPrecision(3, 1);

                entity.HasOne(e => e.Organization)
                    .WithMany()
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.OrganizationId, e.Year, e.Month })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<TalkEventModel>()
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Merchandise>()
                .HasOne<UserModel>()
                .WithMany()
                .HasForeignKey(m => m.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationModel>()
                .HasMany(o => o.Members)
                .WithOne(m => m.Organization)
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // SupportPersonnel Configuration
            modelBuilder.Entity<SupportPersonnel>(entity =>
            {
                entity.ToTable("SupportPersonnel");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.UserId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.RegisteredBy)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.IsActive)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => new { e.IsActive, e.ExperienceLevel })
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.IsDeleted);

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.RegisteredBy)
                    .IsRequired();

                entity.Property(e => e.Skills)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Availability)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.ExperienceLevel)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.Property(e => e.Bio)
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<SupportPersonnel>()
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportPersonnel>()
                .HasOne(sp => sp.RegisteredByUser)
                .WithMany()
                .HasForeignKey(sp => sp.RegisteredBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportPersonnel>()
                .HasMany(sp => sp.Assignments)
                .WithOne(a => a.Personnel)
                .HasForeignKey(a => a.SupportPersonnelId)
                .OnDelete(DeleteBehavior.Restrict);

            // PersonnelSupportRequest Configuration
            modelBuilder.Entity<PersonnelSupportRequest>(entity =>
            {
                entity.ToTable("PersonnelSupportRequests");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.OrganizerId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.EventId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.Status)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.SupportType)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.StartDate)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => new { e.Status, e.StartDate })
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => new { e.OrganizerId, e.Status })
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.OrganizerId)
                    .IsRequired();

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.SupportType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Other");

                entity.Property(e => e.RequiredPersonnel)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.Property(e => e.Requirements)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasDefaultValue("{}");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.FulfillmentNotes)
                    .HasMaxLength(2000);
            });

            modelBuilder.Entity<PersonnelSupportRequest>()
                .HasOne(psr => psr.Organizer)
                .WithMany()
                .HasForeignKey(psr => psr.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelSupportRequest>()
                .HasOne(psr => psr.Event)
                .WithMany()
                .HasForeignKey(psr => psr.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelSupportRequest>()
                .HasOne(psr => psr.ApprovedByUser)
                .WithMany()
                .HasForeignKey(psr => psr.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PersonnelSupportRequest>()
                .HasMany(psr => psr.Assignments)
                .WithOne(a => a.Request)
                .HasForeignKey(a => a.PersonnelSupportRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // PersonnelSupportAssignment Configuration
            modelBuilder.Entity<PersonnelSupportAssignment>(entity =>
            {
                entity.ToTable("PersonnelSupportAssignments");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.PersonnelSupportRequestId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.SupportPersonnelId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.Status)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.AssignedDate)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => new { e.Status, e.AssignedDate })
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.IsDeleted);

                entity.HasIndex(e => new { e.PersonnelSupportRequestId, e.SupportPersonnelId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.PersonnelSupportRequestId)
                    .IsRequired();

                entity.Property(e => e.SupportPersonnelId)
                    .IsRequired();

                entity.Property(e => e.AssignedDate)
                    .IsRequired()
                    .HasDefaultValueSql("now() at time zone 'utc'");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Assigned");

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);
            });

            // Service plan configuration
            modelBuilder.Entity<ServicePlanModel>(entity =>
            {
                entity.ToTable("ServicePlans");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Code)
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.MonthlyPrice)
                    .HasColumnType("numeric(18,2)");

                entity.Property(e => e.YearlyPrice)
                    .HasColumnType("numeric(18,2)");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.Features)
                    .HasColumnType("jsonb");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<UserServicePlanSubscriptionModel>(entity =>
            {
                entity.ToTable("UserServicePlanSubscriptions");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.IsActive })
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18,2)");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ServicePlanSubscriptions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ServicePlan)
                    .WithMany(sp => sp.UserSubscriptions)
                    .HasForeignKey(e => e.ServicePlanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ContractNegotiation)
                    .WithMany(cn => cn.Subscriptions)
                    .HasForeignKey(e => e.ContractNegotiationId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<ContractNegotiationModel>(entity =>
            {
                entity.ToTable("ContractNegotiations");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.CurrentStatus })
                    .HasFilter("\"IsDeleted\" = false");

                entity.Property(e => e.RequestType)
                    .HasMaxLength(50);

                entity.Property(e => e.CurrentStatus)
                    .HasMaxLength(50);

                entity.Property(e => e.ProposedPrice)
                    .HasColumnType("numeric(18,2)");

                entity.Property(e => e.Requirements)
                    .HasColumnType("jsonb");

                entity.Property(e => e.ProposedTerms)
                    .HasColumnType("jsonb");

                entity.Property(e => e.NegotiationNotes)
                    .HasColumnType("jsonb");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ContractNegotiations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ServicePlan)
                    .WithMany(sp => sp.ContractNegotiations)
                    .HasForeignKey(e => e.ServicePlanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // SupportTicket configurations
            modelBuilder.Entity<SupportTicket>(entity =>
            {
                entity.ToTable("SupportTickets");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.TicketNumber)
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.CustomerId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.AssignedToId)
                    .HasFilter("\"AssignedToId\" IS NOT NULL AND \"IsDeleted\" = false");

                entity.HasIndex(e => e.RelatedOrderId)
                    .HasFilter("\"RelatedOrderId\" IS NOT NULL AND \"IsDeleted\" = false");

                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => new { e.CreatedAt, e.LastReplyAt });

                entity.Property(e => e.TicketNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.Tags)
                    .HasMaxLength(500);

                entity.Property(e => e.InternalNotes);

                entity.Property(e => e.SatisfactionRating);

                entity.Property(e => e.SatisfactionComment)
                    .HasMaxLength(1000);

                entity.Property(e => e.MessageCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>()
                    .HasDefaultValue(TicketPriority.Medium);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>()
                    .HasDefaultValue(SupportTicketStatus.Open);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>();
            });

            modelBuilder.Entity<SupportTicket>()
                .HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportTicket>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SupportTicket>()
                .HasOne(t => t.RelatedOrder)
                .WithMany()
                .HasForeignKey(t => t.RelatedOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportTicket>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.SupportTicket)
                .HasForeignKey(m => m.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // SupportTicketMessage configurations
            modelBuilder.Entity<SupportTicketMessage>(entity =>
            {
                entity.ToTable("SupportTicketMessages");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.SupportTicketId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.SenderId)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.SentAt);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.Attachments);

                entity.Property(e => e.IsInternal)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsCustomerMessage)
                    .HasDefaultValue(true);

                entity.Property(e => e.SentAt)
                    .HasDefaultValueSql("now() at time zone 'utc'");
            });

            modelBuilder.Entity<SupportTicketMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation Request Configuration
            modelBuilder.Entity<ConsultationRequest>(entity =>
            {
                entity.ToTable("ConsultationRequests");

                entity.HasIndex(e => e.OrganizerId);
                entity.HasIndex(e => e.ServicePlanId);
                entity.HasIndex(e => e.AssignedStaffId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.PreferredDate);
                entity.HasIndex(e => e.ScheduledDate);

                entity.Property(e => e.ConsultationType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasOne(e => e.Organizer)
                    .WithMany()
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ServicePlan)
                    .WithMany()
                    .HasForeignKey(e => e.ServicePlanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedStaff)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedStaffId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Advertisement configuration
            modelBuilder.Entity<AdvertisementModel>(entity =>
            {
                entity.ToTable("Advertisements");

                entity.HasKey(e => e.Id);

                entity.Ignore(e => e.RemainingBudget);
                entity.Ignore(e => e.ClickThroughRate);
                entity.Ignore(e => e.IsExpired);
                entity.Ignore(e => e.IsScheduledToStart);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.BannerImageUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.TargetUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.CostPerView)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalBudget)
                    .HasPrecision(18, 2);

                entity.Property(e => e.SpentAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TargetAudience)
                    .HasColumnType("jsonb");

                entity.Property(e => e.AdType)
                    .HasConversion<int>();

                entity.Property(e => e.Position)
                    .HasConversion<int>();

                entity.Property(e => e.Status)
                    .HasConversion<int>();

                entity.HasOne(e => e.Advertiser)
                    .WithMany()
                    .HasForeignKey(e => e.AdvertiserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.Position, e.IsActive, e.Status })
                    .HasDatabaseName("IX_Advertisement_Position_Active_Status");

                entity.HasIndex(e => e.AdvertiserId)
                    .HasDatabaseName("IX_Advertisement_AdvertiserId");

                entity.HasIndex(e => new { e.StartDate, e.EndDate })
                    .HasDatabaseName("IX_Advertisement_DateRange");

                entity.HasIndex(e => e.IsDeleted)
                    .HasDatabaseName("IX_Advertisement_IsDeleted");
            });

            // Analytics Entities
            modelBuilder.Entity<KpiSnapshot>(entity =>
            {
                entity.ToTable("KpiSnapshots");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SnapshotDate);
                entity.HasIndex(e => e.Period);

                entity.Property(e => e.Period)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RevenueAnalytics>(entity =>
            {
                entity.ToTable("RevenueAnalytics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.PeriodType, e.Category });

                entity.Property(e => e.PeriodType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserAnalytics>(entity =>
            {
                entity.ToTable("UserAnalytics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.UserType);
                entity.HasIndex(e => e.AcquisitionChannel);
            });

            modelBuilder.Entity<ServiceAnalytics>(entity =>
            {
                entity.ToTable("ServiceAnalytics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PeriodStart, e.PeriodEnd });
                entity.HasIndex(e => e.ServicePlanId);
            });

            modelBuilder.Entity<OrganizationPerformance>(entity =>
            {
                entity.ToTable("OrganizationPerformances");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => new { e.PeriodStart, e.PeriodEnd });
            });

            modelBuilder.Entity<PlatformUsage>(entity =>
            {
                entity.ToTable("PlatformUsages");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.DeviceType);

                entity.Property(e => e.AverageSessionDuration)
                    .HasConversion(
                        v => v.Ticks,
                        v => TimeSpan.FromTicks(v));
            });

            modelBuilder.Entity<GeographicAnalytics>(entity =>
            {
                entity.ToTable("GeographicAnalytics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.PeriodDate);
            });

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.ToTable("ActivityLogs");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OccurredAt);
                entity.HasIndex(e => e.Severity);
                entity.HasIndex(e => e.Category);
            });

            modelBuilder.Entity<PerformanceAlert>(entity =>
            {
                entity.ToTable("PerformanceAlerts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IsResolved);
                entity.HasIndex(e => e.AlertType);
            });

            modelBuilder.Entity<SystemStatistics>(entity =>
            {
                entity.ToTable("SystemStatistics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SnapshotTime);
            });

            modelBuilder.Entity<RevenueBreakdown>(entity =>
            {
                entity.ToTable("RevenueBreakdowns");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PeriodDate);
            });

            modelBuilder.Entity<UserEngagementMetrics>(entity =>
            {
                entity.ToTable("UserEngagementMetrics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.Date });

                entity.Property(e => e.TotalSessionTime)
                    .HasConversion(
                        v => v.Ticks,
                        v => TimeSpan.FromTicks(v));
            });

            modelBuilder.Entity<OrderAnalytics>(entity =>
            {
                entity.ToTable("OrderAnalytics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);

                entity.Property(e => e.AverageProcessingTime)
                    .HasConversion(
                        v => v.Ticks,
                        v => TimeSpan.FromTicks(v));
            });

            modelBuilder.Entity<ConversionFunnel>(entity =>
            {
                entity.ToTable("ConversionFunnels");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);
            });

            modelBuilder.Entity<DashboardConfiguration>(entity =>
            {
                entity.ToTable("DashboardConfigurations");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.DashboardType })
                    .IsUnique();
            });

            modelBuilder.Entity<MentoringRecord>(entity =>
            {
                entity.ToTable("MentoringRecords");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(2000);

                entity.Property(e => e.Topic)
                    .HasMaxLength(200);

                entity.Property(e => e.MeetingLink)
                    .HasMaxLength(500);

                entity.Property(e => e.MeetingPassword)
                    .HasMaxLength(100);

                entity.Property(e => e.Location)
                    .HasMaxLength(500);

                entity.Property(e => e.SessionNotes)
                    .HasColumnType("text");

                entity.Property(e => e.ActionItems)
                    .HasColumnType("text");

                entity.Property(e => e.MenteeProgress)
                    .HasColumnType("text");

                entity.Property(e => e.PrepMaterials)
                    .HasColumnType("text");

                entity.Property(e => e.MenteeFeedback)
                    .HasMaxLength(1000);

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.SessionType)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                // Relationships
                entity.HasOne(e => e.Mentor)
                    .WithMany()
                    .HasForeignKey(e => e.MentorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Mentee)
                    .WithMany()
                    .HasForeignKey(e => e.MenteeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ConsultationRequest)
                    .WithMany()
                    .HasForeignKey(e => e.ConsultationRequestId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes
                entity.HasIndex(e => e.MentorId);
                entity.HasIndex(e => e.MenteeId);
                entity.HasIndex(e => e.SessionDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.MentorId, e.SessionDate });
            });

            // Configure MentoringSessionParticipant
            modelBuilder.Entity<MentoringSessionParticipant>(entity =>
            {
                entity.ToTable("MentoringSessionParticipants");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ParticipantNotes)
                    .HasMaxLength(1000);

                entity.Property(e => e.Feedback)
                    .HasMaxLength(1000);

                // Relationships
                entity.HasOne(e => e.MentoringRecord)
                    .WithMany(m => m.Participants)
                    .HasForeignKey(e => e.MentoringRecordId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes and Constraints
                // PostgreSQL syntax: use double quotes and false instead of 0
                entity.HasIndex(e => new { e.MentoringRecordId, e.UserId })
                    .IsUnique()
                    .HasFilter("\"IsDeleted\" = false");
            });

            // Configure MentoringSessionAttachment
            modelBuilder.Entity<MentoringSessionAttachment>(entity =>
            {
                entity.ToTable("MentoringSessionAttachments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FileType)
                    .HasMaxLength(50);

                entity.Property(e => e.UploadedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                // Relationships
                entity.HasOne(e => e.MentoringRecord)
                    .WithMany(m => m.Attachments)
                    .HasForeignKey(e => e.MentoringRecordId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.MentoringRecordId);
            });

            // Configure MentorAvailability
            modelBuilder.Entity<MentorAvailability>(entity =>
            {
                entity.ToTable("MentorAvailabilities");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DayOfWeek)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                // Relationships
                entity.HasOne(e => e.Mentor)
                    .WithMany()
                    .HasForeignKey(e => e.MentorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.MentorId);
                entity.HasIndex(e => new { e.MentorId, e.DayOfWeek, e.IsRecurring });
            });

            // Configure MentorBlockedTime
            modelBuilder.Entity<MentorBlockedTime>(entity =>
            {
                entity.ToTable("MentorBlockedTimes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Reason)
                    .HasMaxLength(200);

                // Relationships
                entity.HasOne(e => e.Mentor)
                    .WithMany()
                    .HasForeignKey(e => e.MentorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.MentorId);
                entity.HasIndex(e => new { e.MentorId, e.StartDateTime, e.EndDateTime });
            });
            // Configure notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserId")
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasColumnName("Title")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Message)
                    .HasColumnName("Message")
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .HasColumnName("Type")
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(e => e.Priority)
                    .HasColumnName("Priority")
                    .HasConversion<int>()
                    .IsRequired()
                    .HasDefaultValue(NotificationPriority.Normal);

                entity.Property(e => e.RelatedEntityType)
                    .HasColumnName("RelatedEntityType")
                    .HasMaxLength(50);

                entity.Property(e => e.RelatedEntityId)
                    .HasColumnName("RelatedEntityId");

                entity.Property(e => e.ActionUrl)
                    .HasColumnName("ActionUrl")
                    .HasMaxLength(500);

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("ImageUrl")
                    .HasMaxLength(500);

                entity.Property(e => e.Metadata)
                    .HasColumnName("Metadata")
                    .HasColumnType("text");

                entity.Property(e => e.IsRead)
                    .HasColumnName("IsRead")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ReadAt)
                    .HasColumnName("ReadAt");

                entity.Property(e => e.ExpiresAt)
                    .HasColumnName("ExpiresAt");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("CreatedAt")
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("UpdatedAt");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CreatedBy")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("UpdatedBy")
                    .HasMaxLength(100);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("IsDeleted")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.DeletedAt)
                    .HasColumnName("DeletedAt");

                // Relationships
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_Notifications_UserId");

                entity.HasIndex(e => new { e.UserId, e.IsRead })
                    .HasDatabaseName("IX_Notifications_UserId_IsRead")
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                    .HasDatabaseName("IX_Notifications_UserId_CreatedAt")
                    .IsDescending(false, true)
                    .HasFilter("\"IsDeleted\" = false");

                entity.HasIndex(e => e.Type)
                    .HasDatabaseName("IX_Notifications_Type");

                entity.HasIndex(e => e.Priority)
                    .HasDatabaseName("IX_Notifications_Priority");

                entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId })
                    .HasDatabaseName("IX_Notifications_RelatedEntity");

                entity.HasIndex(e => e.ExpiresAt)
                    .HasDatabaseName("IX_Notifications_ExpiresAt")
                    .HasFilter("\"ExpiresAt\" IS NOT NULL AND \"IsDeleted\" = false");

                entity.HasIndex(e => e.IsDeleted)
                    .HasDatabaseName("IX_Notifications_IsDeleted");
            });

            // Apply query filters for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "entity");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var filter = Expression.Lambda(
                        Expression.Not(property),
                        parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                }
            }
        }
    }
}