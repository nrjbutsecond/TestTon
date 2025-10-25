using Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Admin
{
    public interface IActivityTrackingRepo : IRepo<ActivityLog>
    {
        Task<List<ActivityLog>> GetRecentActivitiesAsync(int count);
        Task<List<ActivityLog>> GetActivitiesBySeverityAsync(string severity, int count);
        Task<List<ActivityLog>> GetActivitiesByCategoryAsync(string category, DateTime startDate, DateTime endDate);
        Task LogActivityAsync(string activityType, string description, string severity, string category, string userId = null);

        // Performance Alerts
        Task<List<PerformanceAlert>> GetActiveAlertsAsync();
        Task<List<PerformanceAlert>> GetAlertsByTypeAsync(string alertType);
        Task<bool> ResolveAlertAsync(int alertId, string resolvedBy);
        Task CreateAlertAsync(string alertType, string title, string message, string severity, decimal thresholdValue, decimal actualValue);
    
}
}
