using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AnalyticsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnalyticsBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Run every hour

        public AnalyticsBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AnalyticsBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Analytics Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Analytics Background Service is running at: {time}", DateTimeOffset.Now);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
                        await analyticsService.GenerateAnalyticsSnapshotAsync();
                    }

                    _logger.LogInformation("Analytics snapshot generated successfully at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while generating analytics snapshot.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Analytics Background Service is stopping.");
        }
    }
}