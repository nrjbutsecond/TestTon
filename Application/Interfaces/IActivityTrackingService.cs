using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserLogTracking;
namespace Application.Interfaces
{
    public interface IActivityTrackingService
    {
        Task LogActivityAsync(LogActivityRequest request);
        Task StartSessionAsync(int userId, string? ipAddress = null, string? userAgent = null);
        Task PingSessionAsync(int userId);
        Task EndSessionAsync(int userId);
        Task<UserActivityStatusDto> GetActivityStatusAsync(int userId);
        Task<UserActivityHistoryDto> GetActivityHistoryAsync(int userId, int take = 20);
        Task<List<UserActivityStatusDto>> GetBulkActivityStatusAsync(List<int> userIds);
        Task<int> GetOnlineUsersCountAsync();
        Task CleanupOldLogsAsync(int daysToKeep = 90);
    }
}
