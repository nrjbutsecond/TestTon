
using Application.Helper;
using Application.Interfaces;
using Application.Interfaces.OrganizationServiceInterface;
using Application.Service;
using Domain.Interface;
using Domain.Interface.Admin;
using Domain.Interface.Mentor;
using Domain.Interface.OrganizationRepoFolder;
using Domain.Interface.ServicePlan;
using Infrastructure.Middleware;
using Infrastructure.Repo;
using Infrastructure.Repo.Admin;
using Infrastructure.Repo.Mentor;
using Infrastructure.Repo.OrganizationImp;
using Infrastructure.Repo.ServicePlan;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using Ticket.Application.Interface;
using Ticket.Application.Service;
using Ticket.Domain.Interface;
using Ticket.Infrastructure.Data;
using Ticket.Infrastructure.Repo;
using TON.Configuration;
using TON.Filters;
using TON.Hubs;
using TON.Middleware;
using TON.Services;
namespace TON
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            //database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //
            // Register HTTP clients
            builder.Services.AddHttpClient<GhnShippingProviderService>(client =>
            {
                var settings = builder.Configuration.GetSection("Shipping:Providers:GHN").Get<GhnSettings>();
           //     client.BaseAddress = new Uri(settings?.BaseUrl ?? "https://online-gateway.ghn.vn/shiip///public-api"); Production
                client.BaseAddress = new Uri(settings?.BaseUrl ?? "https://dev.ghn.vn/api"); //dev
            });

            

            //repo and unitofwork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IOrganizationRepo, OrganizationRepo>();
            builder.Services.AddScoped<IOrganizationMemberRepo, OrganizationMemberRepo>();
            builder.Services.AddScoped<IPartnershipApplicationRepo, PartnershipApplicationRepo>();
            builder.Services.AddScoped<IOrganizationActivityRepo, OrganizationActivityRepo>();
            builder.Services.AddScoped<IOrganizationStatisticsRepo, OrganizationStatisticsRepo>();
            builder.Services.AddScoped<IActivityLogRepo, ActivityLogRepo>();
            builder.Services.AddScoped<ISessionRepo, SessionRepo>();
            builder.Services.AddScoped<IActivityTrackingRepo, ActivityTrackingRepo>();
            builder.Services.AddScoped<IAnalyticsDataRepo, AnalyticsDataRepo>();
            builder.Services.AddScoped<IPerformanceMonitoringRepo, PerformanceMonitoringRepo>();
            builder.Services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            builder.Services.AddScoped<IMerchandiseRepo, MerchandiseRepo>();
            builder.Services.AddScoped<IMerchandiseVariantRepo, MerchandiseVariantRepo>();
            builder.Services.AddScoped<IServicePlanRepo, ServicePlanRepo>();
            builder.Services.AddScoped<IUserServicePlanSubscriptionRepo, UserServicePlanSubscriptionRepo>();
            builder.Services.AddScoped<IContractNegotiationRepo, ContractNegotiationRepo>();
            builder.Services.AddScoped<IConsultationRequestRepo, ConsultationRequestRepo>();
            builder.Services.AddScoped<ISupportTicketRepo, SupportTicketRepo>();
            builder.Services.AddScoped<ISupportTicketMessageRepo, SupportTicketMessageRepo>();
            builder.Services.AddScoped<IMentoringSessionRepo, MentoringSessionRepo>();
            builder.Services.AddScoped<IMentorAvailabilityRepo, MentorAvailabilityRepo>();
            builder.Services.AddScoped<IMentorBlockedTimeRepo, MentorBlockedTimeRepo>();
            builder.Services.AddScoped<IMentoringParticipantRepo, MentoringParticipantRepo>();
            builder.Services.AddScoped<IMentoringAttachmentRepo, MentoringAttachmentRepo>();
            builder.Services.AddScoped<INotificationRepo, NotificationRepo>();

            //

            //
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ITalkEventService, TalkEventService>();
            builder.Services.AddScoped<IWorkshopService, WorkshopService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IQrCodeService, QrCodeService>();
            builder.Services.AddScoped<IMerchandiseService, MerchandiseService>();
            builder.Services.AddScoped<IShippingConfigService, ShippingConfigService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<LocalShippingProviderService>();
            builder.Services.AddScoped<IShippingProviderFactoryService, ShippingProviderFactoryService>();
            builder.Services.AddScoped<IEnhancedShippingService, EnhancedShippingService>();
            builder.Services.AddScoped<ISponsorService, SponsorService>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IActivityTrackingService, ActivityTrackingService>();
            builder.Services.AddScoped<IActivityTrackingService, ActivityTrackingService>();
            builder.Services.AddScoped<AnalyticsDataSeeder>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IPersonnelSupportService, PersonnelSupportService>();
            builder.Services.AddScoped<IServicePlanService, ServicePlanService>();
            builder.Services.AddScoped<IConsultationRequestService, ConsultationRequestService>();
            builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
            builder.Services.AddScoped<IMentoringService, MentoringService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INotificationHubService, SignalRNotificationHubService>();





            // Background Service
            builder.Services.AddHostedService<AnalyticsBackgroundService>();
            builder.Services.AddHostedService<NotificationReminderService>();

            //
            builder.Services.Configure<QrCodeSettings>(builder.Configuration.GetSection("QrCodeSettings"));
            builder.Services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<QrCodeSettings>>().Value);

            // Configure settings
            builder.Services.Configure<GhnSettings>(
                builder.Configuration.GetSection("Shipping:Providers:GHN"));

            // Add services to the container.
            // Add global model validation
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            });

            // Configure ApiBehaviorOptions
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // We're using custom filter
            });
            //

            // JWT Configuration
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings!.Secret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Controllers with filters
         /*   builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            });
         */
            //

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            //



            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
           // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TON API",
                    Version = "v1",
                    Description = "API for TON Event Management System"
                });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();


            // Middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("SignalRPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseActivityLogger();

            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");
            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
