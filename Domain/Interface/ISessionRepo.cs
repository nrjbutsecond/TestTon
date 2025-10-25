using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ISessionRepo : IRepo<UserSession>
    {
        Task<UserSession?> GetActiveSessionByUserIdAsync(int userId);
        Task<List<UserSession>> GetActiveSessionsByUserIdsAsync(List<int> userIds);
        Task<int> GetOnlineUsersCountAsync(int thresholdMinutes = 5);
        Task<List<UserSession>> GetExpiredSessionsAsync(int thresholdMinutes = 5);
        Task EndAllUserSessionsAsync(int userId);
    }
}
