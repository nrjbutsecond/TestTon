using Application.DTOs.UserLogTracking;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helper;
namespace Application.Service
{
    public class ActivityTrackingService : IActivityTrackingService
    {
        private readonly IActivityLogRepo _activityLogRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IRepo<UserModel> _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityTrackingService> _logger;
        private const int ONLINE_THRESHOLD_MINUTES = 5;

        public ActivityTrackingService(
            IActivityLogRepo activityLogRepo,
            ISessionRepo sessionRepo,
            IRepo<UserModel> userRepo,
            IMapper mapper,
            ILogger<ActivityTrackingService> logger)
        {
            _activityLogRepo = activityLogRepo;
            _sessionRepo = sessionRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task LogActivityAsync(LogActivityRequest request)
        {
            try
            {
                var log = new UserActivityLog
                {
                    UserId = request.UserId,
                    ActivityType = request.ActivityType,
                    Path = request.Path,
                    Method = request.Method,
                    IpAddress = request.IpAddress,
                    UserAgent = request.UserAgent,
                    Timestamp = DateTime.UtcNow
                };

                await _activityLogRepo.AddAsync(log);

                // Auto ping session when logging activity
                await PingSessionAsync(request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity for user {UserId}", request.UserId);
                throw;
            }
        }

        public async Task StartSessionAsync(int userId, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                // End existing sessions
                await _sessionRepo.EndAllUserSessionsAsync(userId);

                // Create new session
                var newSession = new UserSession
                {
                    UserId = userId,
                    StartedAt = DateTime.UtcNow,
                    LastPingAt = DateTime.UtcNow,
                    IsActive = true,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                await _sessionRepo.AddAsync(newSession);

                _logger.LogInformation("Session started for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start session for user {UserId}", userId);
                throw;
            }
        }

        public async Task PingSessionAsync(int userId)
        {
            try
            {
                var activeSession = await _sessionRepo.GetActiveSessionByUserIdAsync(userId);

                if (activeSession != null)
                {
                    activeSession.LastPingAt = DateTime.UtcNow;
                    _sessionRepo.Update(activeSession);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ping session for user {UserId}", userId);
            }
        }

        public async Task EndSessionAsync(int userId)
        {
            try
            {
                await _sessionRepo.EndAllUserSessionsAsync(userId);
                _logger.LogInformation("Session ended for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to end session for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserActivityStatusDto> GetActivityStatusAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found");

            var lastActivity = await _activityLogRepo.GetLastActivityByUserIdAsync(userId);
            var activeSession = await _sessionRepo.GetActiveSessionByUserIdAsync(userId);

            bool isOnline = false;
            if (activeSession != null)
            {
                var timeSinceLastPing = DateTime.UtcNow - activeSession.LastPingAt;
                isOnline = timeSinceLastPing.TotalMinutes <= ONLINE_THRESHOLD_MINUTES;
            }

            return new UserActivityStatusDto
            {
                JoinedDate = user.CreatedAt,
                JoinedFormatted = $"Joined {TimeHelper.FormatDateTime(user.CreatedAt)}",
                LastActivityAt = lastActivity?.Timestamp,
                LastActivityFormatted = lastActivity != null
                    ? $"Last: {TimeHelper.GetRelativeTime(lastActivity.Timestamp)}"
                    : "Never",
                IsOnline = isOnline,
                OnlineStatus = isOnline ? "Online" : "Offline"
            };
        }

        public async Task<UserActivityHistoryDto> GetActivityHistoryAsync(int userId, int take = 20)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found");

            var status = await GetActivityStatusAsync(userId);
            var logs = await _activityLogRepo.GetUserActivitiesAsync(userId, take);
            var totalCount = await _activityLogRepo.GetTotalActivitiesCountAsync(userId);

            return new UserActivityHistoryDto
            {
                UserId = userId,
                UserName = user.FullName,
                Status = status,
                RecentActivities = _mapper.Map<List<ActivityLogDto>>(logs), // ✅ Dùng AutoMapper
                TotalActivitiesCount = totalCount
            };
        }

        public async Task<List<UserActivityStatusDto>> GetBulkActivityStatusAsync(List<int> userIds)
        {
            var result = new List<UserActivityStatusDto>();

            // Get all sessions at once for efficiency
            var sessions = await _sessionRepo.GetActiveSessionsByUserIdsAsync(userIds);
            var sessionDict = sessions.ToDictionary(s => s.UserId, s => s);

            foreach (var userId in userIds)
            {
                try
                {
                    var user = await _userRepo.GetByIdAsync(userId);
                    if (user == null) continue;

                    var lastActivity = await _activityLogRepo.GetLastActivityByUserIdAsync(userId);

                    bool isOnline = false;
                    if (sessionDict.TryGetValue(userId, out var session))
                    {
                        var timeSinceLastPing = DateTime.UtcNow - session.LastPingAt;
                        isOnline = timeSinceLastPing.TotalMinutes <= ONLINE_THRESHOLD_MINUTES;
                    }

                    result.Add(new UserActivityStatusDto
                    {
                        JoinedDate = user.CreatedAt,
                        JoinedFormatted = $"Joined {TimeHelper.FormatDateTime(user.CreatedAt)}",
                        LastActivityAt = lastActivity?.Timestamp,
                        LastActivityFormatted = lastActivity != null
                            ? $"Last: {TimeHelper.GetRelativeTime(lastActivity.Timestamp)}"
                            : "Never",
                        IsOnline = isOnline,
                        OnlineStatus = isOnline ? "Online" : "Offline"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting status for user {UserId}", userId);
                }
            }

            return result;
        }

        public async Task<int> GetOnlineUsersCountAsync()
        {
            return await _sessionRepo.GetOnlineUsersCountAsync(ONLINE_THRESHOLD_MINUTES);
        }

        public async Task CleanupOldLogsAsync(int daysToKeep = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            await _activityLogRepo.DeleteOldLogsAsync(cutoffDate);
            _logger.LogInformation("Cleaned up activity logs older than {Days} days", daysToKeep);
        }
    }

}