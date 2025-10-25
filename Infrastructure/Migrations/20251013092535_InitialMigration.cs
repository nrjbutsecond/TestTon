using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversionFunnels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FunnelStage = table.Column<string>(type: "text", nullable: false),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DropOffRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UserSegment = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionFunnels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DashboardType = table.Column<string>(type: "text", nullable: false),
                    WidgetConfiguration = table.Column<string>(type: "text", nullable: false),
                    DateRangeDefault = table.Column<string>(type: "text", nullable: false),
                    ShowRevenueTrend = table.Column<bool>(type: "boolean", nullable: false),
                    ShowUserGrowth = table.Column<bool>(type: "boolean", nullable: false),
                    ShowGeographic = table.Column<bool>(type: "boolean", nullable: false),
                    ShowTopServices = table.Column<bool>(type: "boolean", nullable: false),
                    ShowAlerts = table.Column<bool>(type: "boolean", nullable: false),
                    Theme = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeographicAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "text", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    UserPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Revenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OrderCount = table.Column<int>(type: "integer", nullable: false),
                    PeriodDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GrowthRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActiveOrganizations = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeographicAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KpiSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SnapshotDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalActiveUsers = table.Column<int>(type: "integer", nullable: false),
                    TotalOrders = table.Column<int>(type: "integer", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RevenueGrowthPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UserGrowthPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OrderGrowthPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ConversionGrowthPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Period = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalOrders = table.Column<int>(type: "integer", nullable: false),
                    CompletedOrders = table.Column<int>(type: "integer", nullable: false),
                    PendingOrders = table.Column<int>(type: "integer", nullable: false),
                    CancelledOrders = table.Column<int>(type: "integer", nullable: false),
                    CompletionRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CancellationRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AverageOrderValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AverageProcessingTime = table.Column<long>(type: "bigint", nullable: false),
                    TopCategory = table.Column<string>(type: "text", nullable: false),
                    RefundRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPerformances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationName = table.Column<string>(type: "text", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EventCount = table.Column<int>(type: "integer", nullable: false),
                    TicketsSold = table.Column<int>(type: "integer", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPerformances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Website = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    PartnershipTier = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LicenseActiveUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FoundedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(18,2)", precision: 3, scale: 1, nullable: false),
                    TotalEvents = table.Column<int>(type: "integer", nullable: false),
                    ActivePartners = table.Column<int>(type: "integer", nullable: false),
                    TotalAttendees = table.Column<int>(type: "integer", nullable: false),
                    MonthlyRevenue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertType = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedBy = table.Column<string>(type: "text", nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MetricName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceType = table.Column<string>(type: "text", nullable: false),
                    SessionCount = table.Column<int>(type: "integer", nullable: false),
                    UniqueUsers = table.Column<int>(type: "integer", nullable: false),
                    UsagePercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AverageSessionDuration = table.Column<long>(type: "bigint", nullable: false),
                    PageViewsPerSession = table.Column<int>(type: "integer", nullable: false),
                    BounceRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Browser = table.Column<string>(type: "text", nullable: false),
                    OperatingSystem = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUsages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevenueAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    PeriodType = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    TransactionCount = table.Column<int>(type: "integer", nullable: false),
                    AverageOrderValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Category = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevenueBreakdowns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PeriodDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ServiceRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WorkshopRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EventRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MerchandiseRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ConsultationRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MentoringRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AdvertisementRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PeriodType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueBreakdowns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServicePlanId = table.Column<int>(type: "integer", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: false),
                    OrderCount = table.Column<int>(type: "integer", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GrowthPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UniqueCustomers = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaxEvents = table.Column<int>(type: "integer", nullable: false),
                    MaxMerchandiseItems = table.Column<int>(type: "integer", nullable: false),
                    IncludesConsultation = table.Column<bool>(type: "boolean", nullable: false),
                    IncludesPersonnelSupport = table.Column<bool>(type: "boolean", nullable: false),
                    ConsultationHours = table.Column<int>(type: "integer", nullable: false),
                    Features = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "text", nullable: true),
                    Ward = table.Column<string>(type: "text", nullable: true),
                    FreeShippingThreshold = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BulkOrderThreshold = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BulkOrderExtraFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PreferredProvider = table.Column<string>(type: "text", nullable: true),
                    MarkupPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MarkupFixedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MinShippingFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaxShippingFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BaseFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PromotionStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PromotionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Logo = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "text", maxLength: 2000, nullable: true),
                    ContactPerson = table.Column<string>(type: "text", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "text", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "text", maxLength: 20, nullable: true),
                    SponsorshipLevel = table.Column<string>(type: "text", maxLength: 20, nullable: false, defaultValue: "Bronze"),
                    ContributionAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ContractStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Benefits = table.Column<string>(type: "jsonb", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                    table.CheckConstraint("CK_Sponsors_ContractDates", "\"ContractEndDate\" IS NULL OR \"ContractStartDate\" IS NULL OR \"ContractEndDate\" >= \"ContractStartDate\"");
                    table.CheckConstraint("CK_Sponsors_ContributionAmount", "\"ContributionAmount\" >= 0");
                    table.CheckConstraint("CK_Sponsors_SponsorshipLevel", "\"SponsorshipLevel\" IN ('Bronze', 'Silver', 'Gold', 'Platinum')");
                });

            migrationBuilder.CreateTable(
                name: "SystemStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SnapshotTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalUsers = table.Column<int>(type: "integer", nullable: false),
                    ActiveOrders = table.Column<int>(type: "integer", nullable: false),
                    MonthlyRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalEvents = table.Column<int>(type: "integer", nullable: false),
                    TotalWorkshops = table.Column<int>(type: "integer", nullable: false),
                    TotalMerchandise = table.Column<int>(type: "integer", nullable: false),
                    TotalOrganizations = table.Column<int>(type: "integer", nullable: false),
                    SystemUptime = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ApiCallCount = table.Column<int>(type: "integer", nullable: false),
                    AverageResponseTime = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewUsers = table.Column<int>(type: "integer", nullable: false),
                    ActiveUsers = table.Column<int>(type: "integer", nullable: false),
                    ReturnUsers = table.Column<int>(type: "integer", nullable: false),
                    ChurnedUsers = table.Column<int>(type: "integer", nullable: false),
                    RetentionRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrowthRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    AcquisitionChannel = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEngagementMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoginCount = table.Column<int>(type: "integer", nullable: false),
                    TotalSessionTime = table.Column<long>(type: "bigint", nullable: false),
                    PageViews = table.Column<int>(type: "integer", nullable: false),
                    ActionsPerformed = table.Column<int>(type: "integer", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysSinceLastActivity = table.Column<int>(type: "integer", nullable: false),
                    EngagementLevel = table.Column<string>(type: "text", nullable: false),
                    EngagementScore = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEngagementMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "text", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Enthusiast"),
                    ServicePlan = table.Column<string>(type: "text", maxLength: 50, nullable: false, defaultValue: "Free"),
                    ServicePlanExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPartneredOrganizer = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    RefreshToken = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EmailConfirmationToken = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    EmailConfirmationTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    TotalEvents = table.Column<int>(type: "integer", nullable: false),
                    TotalAttendees = table.Column<int>(type: "integer", nullable: false),
                    Revenue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NewMembers = table.Column<int>(type: "integer", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(18,2)", precision: 3, scale: 1, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationStatistics_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdvertiserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    BannerImageUrl = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    TargetUrl = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    AdType = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ClickCount = table.Column<int>(type: "integer", nullable: false),
                    CostPerView = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalBudget = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TargetAudience = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advertisements_Users_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Author = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    FeaturedImage = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    MetaTitle = table.Column<string>(type: "text", maxLength: 60, nullable: true),
                    MetaDescription = table.Column<string>(type: "text", maxLength: 160, nullable: true),
                    UserModelId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Users_Author",
                        column: x => x.Author,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_Users_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizerId = table.Column<int>(type: "integer", nullable: false),
                    ServicePlanId = table.Column<int>(type: "integer", nullable: false),
                    ConsultationType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    PreferredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    AssignedStaffId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", maxLength: 2000, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ServicePlanModelId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_ServicePlans_ServicePlanId",
                        column: x => x.ServicePlanId,
                        principalTable: "ServicePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_ServicePlans_ServicePlanModelId",
                        column: x => x.ServicePlanModelId,
                        principalTable: "ServicePlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_Users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractNegotiations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ServicePlanId = table.Column<int>(type: "integer", nullable: false),
                    RequestType = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    CurrentStatus = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Requirements = table.Column<string>(type: "jsonb", nullable: false),
                    ProposedTerms = table.Column<string>(type: "jsonb", nullable: false),
                    NegotiationNotes = table.Column<string>(type: "jsonb", nullable: false),
                    ProposedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ContractDuration = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HandledBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractNegotiations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractNegotiations_ServicePlans_ServicePlanId",
                        column: x => x.ServicePlanId,
                        principalTable: "ServicePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractNegotiations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentorAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MentorId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    RecurringEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SpecificDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorAvailabilities_Users_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorBlockedTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MentorId = table.Column<int>(type: "integer", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", maxLength: 200, nullable: true),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorBlockedTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorBlockedTimes_Users_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Merchandises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerId = table.Column<int>(type: "integer", nullable: true),
                    SKU = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "integer", nullable: false),
                    IsOfficial = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Images = table.Column<string>(type: "jsonb", nullable: true),
                    SellerId1 = table.Column<int>(type: "integer", nullable: true),
                    OrganizationModelId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchandises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merchandises_Organizations_OrganizationModelId",
                        column: x => x.OrganizationModelId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Merchandises_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Merchandises_Users_SellerId1",
                        column: x => x.SellerId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ShippingAddress = table.Column<string>(type: "jsonb", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationActivities_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartnershipApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    RequestedTier = table.Column<int>(type: "integer", nullable: false),
                    ApplicationReason = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    ReviewedByUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnershipApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnershipApplications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnershipApplications_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SupportPersonnel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RegisteredBy = table.Column<int>(type: "integer", nullable: false),
                    Skills = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    Availability = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ExperienceLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Bio = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportPersonnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportPersonnel_Users_RegisteredBy",
                        column: x => x.RegisteredBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportPersonnel_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TalkEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizerId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    MaxAttendees = table.Column<int>(type: "integer", nullable: false),
                    HasTicketSale = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BannerImage = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    ThumbnailImage = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CancellationReason = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    OrganizationModelId = table.Column<int>(type: "integer", nullable: true),
                    UserModelId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 255, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalkEvents_Organizations_OrganizationModelId",
                        column: x => x.OrganizationModelId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TalkEvents_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TalkEvents_Users_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ActivityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastPingAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    OnlineLink = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    IsOfficial = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizerId = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    CurrentParticipants = table.Column<int>(type: "integer", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", maxLength: 50, nullable: false, defaultValue: 0),
                    CancellationReason = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workshops_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleArticleTags",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleArticleTags", x => new { x.ArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ArticleArticleTags_ArticleTags_TagId",
                        column: x => x.TagId,
                        principalTable: "ArticleTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleArticleTags_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentoringRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MentorId = table.Column<int>(type: "integer", nullable: false),
                    MenteeId = table.Column<int>(type: "integer", nullable: true),
                    ConsultationRequestId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", maxLength: 2000, nullable: true),
                    SessionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    MeetingLink = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    MeetingPassword = table.Column<string>(type: "text", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Topic = table.Column<string>(type: "text", maxLength: 200, nullable: true),
                    SessionNotes = table.Column<string>(type: "text", nullable: true),
                    ActionItems = table.Column<string>(type: "text", nullable: true),
                    MenteeProgress = table.Column<string>(type: "text", nullable: true),
                    PrepMaterials = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    MenteeFeedback = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    NextSessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextSessionId = table.Column<int>(type: "integer", nullable: true),
                    ReminderSent = table.Column<bool>(type: "boolean", nullable: false),
                    ReminderSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    CurrentParticipants = table.Column<int>(type: "integer", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoringRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentoringRecords_ConsultationRequests_ConsultationRequestId",
                        column: x => x.ConsultationRequestId,
                        principalTable: "ConsultationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MentoringRecords_Users_MenteeId",
                        column: x => x.MenteeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MentoringRecords_Users_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserServicePlanSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ServicePlanId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    ContractNegotiationId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServicePlanSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserServicePlanSubscriptions_ContractNegotiations_ContractN~",
                        column: x => x.ContractNegotiationId,
                        principalTable: "ContractNegotiations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserServicePlanSubscriptions_ServicePlans_ServicePlanId",
                        column: x => x.ServicePlanId,
                        principalTable: "ServicePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserServicePlanSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MerchandiseVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchandiseId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    PriceModifier = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchandiseVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchandiseVariants_Merchandises_MerchandiseId",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketNumber = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    AssignedToId = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Medium"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    RelatedOrderId = table.Column<int>(type: "integer", nullable: true),
                    MessageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FirstResponseAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastReplyAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SatisfactionRating = table.Column<int>(type: "integer", nullable: true),
                    SatisfactionComment = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    InternalNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Orders_RelatedOrderId",
                        column: x => x.RelatedOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSupportRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizerId = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    SupportType = table.Column<string>(type: "text", maxLength: 50, nullable: false, defaultValue: "Other"),
                    RequiredPersonnel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Requirements = table.Column<string>(type: "text", maxLength: 4000, nullable: false, defaultValue: "{}"),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    FulfillmentNotes = table.Column<string>(type: "text", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSupportRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSupportRequests_TalkEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "TalkEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelSupportRequests_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PersonnelSupportRequests_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TalkEventId = table.Column<int>(type: "integer", nullable: true),
                    WorkshopId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: false),
                    SoldQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    SaleStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SaleEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReasonDelete = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.CheckConstraint("CK_TicketType_OneEventType", "(\"TalkEventId\" IS NOT NULL AND \"WorkshopId\" IS NULL) OR (\"TalkEventId\" IS NULL AND \"WorkshopId\" IS NOT NULL)");
                    table.CheckConstraint("CK_TicketType_Price", "\"Price\" >= 0");
                    table.CheckConstraint("CK_TicketType_Quantities", "\"SoldQuantity\" >= 0 AND \"MaxQuantity\" > 0 AND \"SoldQuantity\" <= \"MaxQuantity\"");
                    table.CheckConstraint("CK_TicketType_SaleDates", "\"SaleEndDate\" > \"SaleStartDate\"");
                    table.ForeignKey(
                        name: "FK_TicketTypes_TalkEvents_TalkEventId",
                        column: x => x.TalkEventId,
                        principalTable: "TalkEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketTypes_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentoringSessionAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MentoringRecordId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedBy = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoringSessionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentoringSessionAttachments_MentoringRecords_MentoringRecor~",
                        column: x => x.MentoringRecordId,
                        principalTable: "MentoringRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentoringSessionParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MentoringRecordId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HasJoined = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeftAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParticipantNotes = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    Feedback = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoringSessionParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentoringSessionParticipants_MentoringRecords_MentoringReco~",
                        column: x => x.MentoringRecordId,
                        principalTable: "MentoringRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentoringSessionParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    MerchandiseId = table.Column<int>(type: "integer", nullable: false),
                    VariantId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_MerchandiseVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "MerchandiseVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_Merchandises_MerchandiseId",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    MerchandiseId = table.Column<int>(type: "integer", nullable: false),
                    VariantId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_MerchandiseVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "MerchandiseVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Merchandises_MerchandiseId",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupportTicketId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Attachments = table.Column<string>(type: "text", nullable: true),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCustomerMessage = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTicketMessages_SupportTickets_SupportTicketId",
                        column: x => x.SupportTicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportTicketMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSupportAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonnelSupportRequestId = table.Column<int>(type: "integer", nullable: false),
                    SupportPersonnelId = table.Column<int>(type: "integer", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Status = table.Column<string>(type: "text", maxLength: 50, nullable: false, defaultValue: "Assigned"),
                    Notes = table.Column<string>(type: "text", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSupportAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSupportAssignments_PersonnelSupportRequests_Person~",
                        column: x => x.PersonnelSupportRequestId,
                        principalTable: "PersonnelSupportRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelSupportAssignments_SupportPersonnel_SupportPersonn~",
                        column: x => x.SupportPersonnelId,
                        principalTable: "SupportPersonnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TicketableId = table.Column<int>(type: "integer", nullable: false),
                    TicketableType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TicketTypeId = table.Column<int>(type: "integer", nullable: false),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    QRCode = table.Column<string>(type: "text", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Reserved"),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReasonDelete = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    WorkshopModelId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.CheckConstraint("CK_Ticket_Status", "\"Status\" IN ('Reserved', 'Paid', 'Used', 'Cancelled', 'Expired')");
                    table.CheckConstraint("CK_Ticket_TicketableType", "\"TicketableType\" IN ('TalkEvent', 'Workshop')");
                    table.CheckConstraint("CK_Ticket_ValidDates", "\"ValidUntil\" > \"ValidFrom\"");
                    table.ForeignKey(
                        name: "FK_Tickets_TalkEvents_TicketableId",
                        column: x => x.TicketableId,
                        principalTable: "TalkEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Workshops_WorkshopModelId",
                        column: x => x.WorkshopModelId,
                        principalTable: "Workshops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MerchandiseId = table.Column<int>(type: "integer", nullable: false),
                    OrderItemId = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "text", maxLength: 2000, nullable: false),
                    Images = table.Column<string>(type: "jsonb", nullable: true),
                    IsVerifiedPurchase = table.Column<bool>(type: "boolean", nullable: false),
                    HelpfulCount = table.Column<int>(type: "integer", nullable: false),
                    UnhelpfulCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Merchandises_MerchandiseId",
                        column: x => x.MerchandiseId,
                        principalTable: "Merchandises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    IsSellerResponse = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewResponses_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewResponses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Category",
                table: "ActivityLogs",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_OccurredAt",
                table: "ActivityLogs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Severity",
                table: "ActivityLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_AdvertiserId",
                table: "Advertisements",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_DateRange",
                table: "Advertisements",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_IsDeleted",
                table: "Advertisements",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_Position_Active_Status",
                table: "Advertisements",
                columns: new[] { "Position", "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleArticleTags_ArticleId",
                table: "ArticleArticleTags",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleArticleTags_TagId",
                table: "ArticleArticleTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Author",
                table: "Articles",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PublishedAt",
                table: "Articles",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                table: "Articles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Status",
                table: "Articles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UserModelId",
                table: "Articles",
                column: "UserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_Name",
                table: "ArticleTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_Slug",
                table: "ArticleTags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_MerchandiseId_VariantId",
                table: "CartItems",
                columns: new[] { "CartId", "MerchandiseId", "VariantId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MerchandiseId",
                table: "CartItems",
                column: "MerchandiseId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ExpiresAt",
                table: "Carts",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_AssignedStaffId",
                table: "ConsultationRequests",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_OrganizerId",
                table: "ConsultationRequests",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_PreferredDate",
                table: "ConsultationRequests",
                column: "PreferredDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_ScheduledDate",
                table: "ConsultationRequests",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_ServicePlanId",
                table: "ConsultationRequests",
                column: "ServicePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_ServicePlanModelId",
                table: "ConsultationRequests",
                column: "ServicePlanModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_Status",
                table: "ConsultationRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContractNegotiations_ServicePlanId",
                table: "ContractNegotiations",
                column: "ServicePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractNegotiations_UserId_CurrentStatus",
                table: "ContractNegotiations",
                columns: new[] { "UserId", "CurrentStatus" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionFunnels_Date",
                table: "ConversionFunnels",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardConfigurations_UserId_DashboardType",
                table: "DashboardConfigurations",
                columns: new[] { "UserId", "DashboardType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeographicAnalytics_City",
                table: "GeographicAnalytics",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_GeographicAnalytics_PeriodDate",
                table: "GeographicAnalytics",
                column: "PeriodDate");

            migrationBuilder.CreateIndex(
                name: "IX_KpiSnapshots_Period",
                table: "KpiSnapshots",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_KpiSnapshots_SnapshotDate",
                table: "KpiSnapshots",
                column: "SnapshotDate");

            migrationBuilder.CreateIndex(
                name: "IX_MentorAvailabilities_MentorId",
                table: "MentorAvailabilities",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorAvailabilities_MentorId_DayOfWeek_IsRecurring",
                table: "MentorAvailabilities",
                columns: new[] { "MentorId", "DayOfWeek", "IsRecurring" });

            migrationBuilder.CreateIndex(
                name: "IX_MentorBlockedTimes_MentorId",
                table: "MentorBlockedTimes",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorBlockedTimes_MentorId_StartDateTime_EndDateTime",
                table: "MentorBlockedTimes",
                columns: new[] { "MentorId", "StartDateTime", "EndDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_ConsultationRequestId",
                table: "MentoringRecords",
                column: "ConsultationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_MenteeId",
                table: "MentoringRecords",
                column: "MenteeId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_MentorId",
                table: "MentoringRecords",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_MentorId_SessionDate",
                table: "MentoringRecords",
                columns: new[] { "MentorId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_SessionDate",
                table: "MentoringRecords",
                column: "SessionDate");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringRecords_Status",
                table: "MentoringRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringSessionAttachments_MentoringRecordId",
                table: "MentoringSessionAttachments",
                column: "MentoringRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringSessionParticipants_MentoringRecordId_UserId",
                table: "MentoringSessionParticipants",
                columns: new[] { "MentoringRecordId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_MentoringSessionParticipants_UserId",
                table: "MentoringSessionParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_Category",
                table: "Merchandises",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_IsActive_IsOfficial",
                table: "Merchandises",
                columns: new[] { "IsActive", "IsOfficial" });

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_OrganizationModelId",
                table: "Merchandises",
                column: "OrganizationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_SellerId",
                table: "Merchandises",
                column: "SellerId",
                filter: "\"SellerId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_SellerId1",
                table: "Merchandises",
                column: "SellerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Merchandises_SKU",
                table: "Merchandises",
                column: "SKU",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_MerchandiseVariants_MerchandiseId_Name_Value",
                table: "MerchandiseVariants",
                columns: new[] { "MerchandiseId", "Name", "Value" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAnalytics_Date",
                table: "OrderAnalytics",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MerchandiseId",
                table: "OrderItems",
                column: "MerchandiseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_VariantId",
                table: "OrderItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationActivities_OrganizationId_ActivityDate",
                table: "OrganizationActivities",
                columns: new[] { "OrganizationId", "ActivityDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationActivities_UserId",
                table: "OrganizationActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_OrganizationId",
                table: "OrganizationMembers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_UserId_OrganizationId",
                table: "OrganizationMembers",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPerformances_OrganizationId",
                table: "OrganizationPerformances",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPerformances_PeriodStart_PeriodEnd",
                table: "OrganizationPerformances",
                columns: new[] { "PeriodStart", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Email",
                table: "Organizations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Status_PartnershipTier",
                table: "Organizations",
                columns: new[] { "Status", "PartnershipTier" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStatistics_OrganizationId_Year_Month",
                table: "OrganizationStatistics",
                columns: new[] { "OrganizationId", "Year", "Month" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PartnershipApplications_ApplicationDate",
                table: "PartnershipApplications",
                column: "ApplicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_PartnershipApplications_OrganizationId_Status",
                table: "PartnershipApplications",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PartnershipApplications_ReviewedByUserId",
                table: "PartnershipApplications",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceAlerts_AlertType",
                table: "PerformanceAlerts",
                column: "AlertType");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceAlerts_IsResolved",
                table: "PerformanceAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_AssignedDate",
                table: "PersonnelSupportAssignments",
                column: "AssignedDate",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_IsDeleted",
                table: "PersonnelSupportAssignments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_PersonnelSupportRequestId",
                table: "PersonnelSupportAssignments",
                column: "PersonnelSupportRequestId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_PersonnelSupportRequestId_Suppo~",
                table: "PersonnelSupportAssignments",
                columns: new[] { "PersonnelSupportRequestId", "SupportPersonnelId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_Status",
                table: "PersonnelSupportAssignments",
                column: "Status",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_Status_AssignedDate",
                table: "PersonnelSupportAssignments",
                columns: new[] { "Status", "AssignedDate" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportAssignments_SupportPersonnelId",
                table: "PersonnelSupportAssignments",
                column: "SupportPersonnelId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_ApprovedBy",
                table: "PersonnelSupportRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_EventId",
                table: "PersonnelSupportRequests",
                column: "EventId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_IsDeleted",
                table: "PersonnelSupportRequests",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_OrganizerId",
                table: "PersonnelSupportRequests",
                column: "OrganizerId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_OrganizerId_Status",
                table: "PersonnelSupportRequests",
                columns: new[] { "OrganizerId", "Status" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_StartDate",
                table: "PersonnelSupportRequests",
                column: "StartDate",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_Status",
                table: "PersonnelSupportRequests",
                column: "Status",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_Status_StartDate",
                table: "PersonnelSupportRequests",
                columns: new[] { "Status", "StartDate" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSupportRequests_SupportType",
                table: "PersonnelSupportRequests",
                column: "SupportType",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUsages_Date",
                table: "PlatformUsages",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUsages_DeviceType",
                table: "PlatformUsages",
                column: "DeviceType");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueAnalytics_Date",
                table: "RevenueAnalytics",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueAnalytics_PeriodType_Category",
                table: "RevenueAnalytics",
                columns: new[] { "PeriodType", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_RevenueBreakdowns_PeriodDate",
                table: "RevenueBreakdowns",
                column: "PeriodDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_ReviewId",
                table: "ReviewResponses",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_UserId",
                table: "ReviewResponses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsVerifiedPurchase",
                table: "Reviews",
                column: "IsVerifiedPurchase");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MerchandiseId",
                table: "Reviews",
                column: "MerchandiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderItemId",
                table: "Reviews",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_MerchandiseId",
                table: "Reviews",
                columns: new[] { "UserId", "MerchandiseId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId",
                table: "ReviewVotes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_UserId_ReviewId",
                table: "ReviewVotes",
                columns: new[] { "UserId", "ReviewId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAnalytics_PeriodStart_PeriodEnd",
                table: "ServiceAnalytics",
                columns: new[] { "PeriodStart", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAnalytics_ServicePlanId",
                table: "ServiceAnalytics",
                column: "ServicePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePlans_Code",
                table: "ServicePlans",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingConfigs_City_IsActive",
                table: "ShippingConfigs",
                columns: new[] { "City", "IsActive" },
                unique: true,
                filter: "\"IsActive\" = true AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingConfigs_IsDefault_IsActive",
                table: "ShippingConfigs",
                columns: new[] { "IsDefault", "IsActive" },
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_ContactEmail",
                table: "Sponsors",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_DisplayOrder",
                table: "Sponsors",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_IsActive_IsDeleted",
                table: "Sponsors",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Name",
                table: "Sponsors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_SponsorshipLevel",
                table: "Sponsors",
                column: "SponsorshipLevel");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPersonnel_IsActive",
                table: "SupportPersonnel",
                column: "IsActive",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPersonnel_IsActive_ExperienceLevel",
                table: "SupportPersonnel",
                columns: new[] { "IsActive", "ExperienceLevel" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPersonnel_IsDeleted",
                table: "SupportPersonnel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPersonnel_RegisteredBy",
                table: "SupportPersonnel",
                column: "RegisteredBy",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPersonnel_UserId",
                table: "SupportPersonnel",
                column: "UserId",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketMessages_SenderId",
                table: "SupportTicketMessages",
                column: "SenderId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketMessages_SentAt",
                table: "SupportTicketMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketMessages_SupportTicketId",
                table: "SupportTicketMessages",
                column: "SupportTicketId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssignedToId",
                table: "SupportTickets",
                column: "AssignedToId",
                filter: "\"AssignedToId\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Category",
                table: "SupportTickets",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CreatedAt_LastReplyAt",
                table: "SupportTickets",
                columns: new[] { "CreatedAt", "LastReplyAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CustomerId",
                table: "SupportTickets",
                column: "CustomerId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Priority",
                table: "SupportTickets",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_RelatedOrderId",
                table: "SupportTickets",
                column: "RelatedOrderId",
                filter: "\"RelatedOrderId\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Status",
                table: "SupportTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TicketNumber",
                table: "SupportTickets",
                column: "TicketNumber",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SystemStatistics_SnapshotTime",
                table: "SystemStatistics",
                column: "SnapshotTime");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_OrganizationModelId",
                table: "TalkEvents",
                column: "OrganizationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_OrganizerId",
                table: "TalkEvents",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_StartDate",
                table: "TalkEvents",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_Status",
                table: "TalkEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_Status_StartDate",
                table: "TalkEvents",
                columns: new[] { "Status", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TalkEvents_UserModelId",
                table: "TalkEvents",
                column: "UserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Guid",
                table: "Tickets",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_QRCode",
                table: "Tickets",
                column: "QRCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Status",
                table: "Tickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Ticketable",
                table: "Tickets",
                columns: new[] { "TicketableId", "TicketableType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Ticketable_Status",
                table: "Tickets",
                columns: new[] { "TicketableId", "TicketableType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId_Status_IsDeleted",
                table: "Tickets",
                columns: new[] { "UserId", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ValidDates",
                table: "Tickets",
                columns: new[] { "ValidFrom", "ValidUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_WorkshopModelId",
                table: "Tickets",
                column: "WorkshopModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_SaleDates",
                table: "TicketTypes",
                columns: new[] { "SaleStartDate", "SaleEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_TalkEventId",
                table: "TicketTypes",
                column: "TalkEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_WorkshopId",
                table: "TicketTypes",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_ActivityType",
                table: "UserActivityLogs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_Timestamp",
                table: "UserActivityLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserId_Timestamp",
                table: "UserActivityLogs",
                columns: new[] { "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnalytics_AcquisitionChannel",
                table: "UserAnalytics",
                column: "AcquisitionChannel");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnalytics_Date",
                table: "UserAnalytics",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnalytics_UserType",
                table: "UserAnalytics",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IX_UserEngagementMetrics_UserId_Date",
                table: "UserEngagementMetrics",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive_EmailConfirmed",
                table: "Users",
                columns: new[] { "IsActive", "EmailConfirmed" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                filter: "\"Phone\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshToken",
                table: "Users",
                column: "RefreshToken",
                filter: "\"RefreshToken\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ServicePlan_ServicePlanExpiry",
                table: "Users",
                columns: new[] { "ServicePlan", "ServicePlanExpiry" });

            migrationBuilder.CreateIndex(
                name: "IX_UserServicePlanSubscriptions_ContractNegotiationId",
                table: "UserServicePlanSubscriptions",
                column: "ContractNegotiationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServicePlanSubscriptions_ServicePlanId",
                table: "UserServicePlanSubscriptions",
                column: "ServicePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServicePlanSubscriptions_UserId_IsActive",
                table: "UserServicePlanSubscriptions",
                columns: new[] { "UserId", "IsActive" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_IsActive_LastPingAt",
                table: "UserSessions",
                columns: new[] { "IsActive", "LastPingAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_LastPingAt",
                table: "UserSessions",
                column: "LastPingAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_IsActive",
                table: "UserSessions",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Workshops_OrganizerId",
                table: "Workshops",
                column: "OrganizerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "ArticleArticleTags");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "ConversionFunnels");

            migrationBuilder.DropTable(
                name: "DashboardConfigurations");

            migrationBuilder.DropTable(
                name: "GeographicAnalytics");

            migrationBuilder.DropTable(
                name: "KpiSnapshots");

            migrationBuilder.DropTable(
                name: "MentorAvailabilities");

            migrationBuilder.DropTable(
                name: "MentorBlockedTimes");

            migrationBuilder.DropTable(
                name: "MentoringSessionAttachments");

            migrationBuilder.DropTable(
                name: "MentoringSessionParticipants");

            migrationBuilder.DropTable(
                name: "OrderAnalytics");

            migrationBuilder.DropTable(
                name: "OrganizationActivities");

            migrationBuilder.DropTable(
                name: "OrganizationMembers");

            migrationBuilder.DropTable(
                name: "OrganizationPerformances");

            migrationBuilder.DropTable(
                name: "OrganizationStatistics");

            migrationBuilder.DropTable(
                name: "PartnershipApplications");

            migrationBuilder.DropTable(
                name: "PerformanceAlerts");

            migrationBuilder.DropTable(
                name: "PersonnelSupportAssignments");

            migrationBuilder.DropTable(
                name: "PlatformUsages");

            migrationBuilder.DropTable(
                name: "RevenueAnalytics");

            migrationBuilder.DropTable(
                name: "RevenueBreakdowns");

            migrationBuilder.DropTable(
                name: "ReviewResponses");

            migrationBuilder.DropTable(
                name: "ReviewVotes");

            migrationBuilder.DropTable(
                name: "ServiceAnalytics");

            migrationBuilder.DropTable(
                name: "ShippingConfigs");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropTable(
                name: "SupportTicketMessages");

            migrationBuilder.DropTable(
                name: "SystemStatistics");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropTable(
                name: "UserAnalytics");

            migrationBuilder.DropTable(
                name: "UserEngagementMetrics");

            migrationBuilder.DropTable(
                name: "UserServicePlanSubscriptions");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "ArticleTags");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "MentoringRecords");

            migrationBuilder.DropTable(
                name: "PersonnelSupportRequests");

            migrationBuilder.DropTable(
                name: "SupportPersonnel");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "ContractNegotiations");

            migrationBuilder.DropTable(
                name: "ConsultationRequests");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "TalkEvents");

            migrationBuilder.DropTable(
                name: "Workshops");

            migrationBuilder.DropTable(
                name: "ServicePlans");

            migrationBuilder.DropTable(
                name: "MerchandiseVariants");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Merchandises");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
