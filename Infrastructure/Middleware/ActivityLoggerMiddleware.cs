using Application.DTOs.UserLogTracking;
using Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Middleware
{
    public class ActivityLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ActivityLoggerMiddleware> _logger;

        public ActivityLoggerMiddleware(
            RequestDelegate next,
            ILogger<ActivityLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IActivityTrackingService activityService)
        {
            await _next(context);

            // Chỉ log nếu user đã login
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        var request = new LogActivityRequest
                        {
                            UserId = userId,
                            ActivityType = DetermineActivityType(context),
                            Path = context.Request.Path.Value,
                            Method = context.Request.Method,
                            IpAddress = GetClientIpAddress(context),
                            UserAgent = context.Request.Headers["User-Agent"].ToString()
                        };

                        // Fire and forget - không block response
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await activityService.LogActivityAsync(request);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Background activity logging failed for user {UserId}", userId);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in activity logger middleware");
                }
            }
        }

        private static string DetermineActivityType(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
            var method = context.Request.Method;

            // Map specific endpoints
            if (path.Contains("/api/auth/login")) return "Login";
            if (path.Contains("/api/auth/logout")) return "Logout";
            if (path.Contains("/api/orders") && method == "POST") return "PlaceOrder";
            if (path.Contains("/api/tickets") && method == "POST") return "PurchaseTicket";
            if (path.Contains("/api/events") && method == "POST") return "CreateEvent";
            if (path.Contains("/api/merchandise") && method == "POST") return "AddMerchandise";
            if (path.Contains("/api/reviews") && method == "POST") return "WriteReview";
            if (path.Contains("/api/cart")) return "CartAction";

            // Generic activity types
            return method switch
            {
                "GET" => "Browse",
                "POST" => "Create",
                "PUT" => "Update",
                "PATCH" => "Update",
                "DELETE" => "Delete",
                _ => "Activity"
            };
        }

        private static string? GetClientIpAddress(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }

    public static class ActivityLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseActivityLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActivityLoggerMiddleware>();
        }
    }
}

