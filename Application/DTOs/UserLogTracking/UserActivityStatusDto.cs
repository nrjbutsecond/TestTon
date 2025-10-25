using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserLogTracking
{
    public class UserActivityStatusDto
    {
        public DateTime JoinedDate { get; set; }
        public string JoinedFormatted { get; set; } = string.Empty; // "Joined 15/03/2024"
        public DateTime? LastActivityAt { get; set; }
        public string LastActivityFormatted { get; set; } = "Never"; // "Last: 2 hours ago"
        public bool IsOnline { get; set; }
        public string OnlineStatus { get; set; } = "Offline"; // "Online", "Offline", "Away"
    }

    /// <summary>
    /// DTO cho activity log item
    /// </summary>
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? Details { get; set; }
    }

    /// <summary>
    /// DTO request để log activity
    /// </summary>
    public class LogActivityRequest
    {
        public int UserId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// DTO response cho activity history
    /// </summary>
    public class UserActivityHistoryDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserActivityStatusDto Status { get; set; } = new();
        public List<ActivityLogDto> RecentActivities { get; set; } = new();
        public int TotalActivitiesCount { get; set; }
    }

    /// <summary>
    /// DTO cho session info
    /// </summary>
    public class UserSessionDto
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastPingAt { get; set; }
        public bool IsActive { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string? DeviceInfo { get; set; }
    }
}
