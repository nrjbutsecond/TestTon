using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IActivityLogRepo : IRepo<UserActivityLog>
    {
        Task<UserActivityLog?> GetLastActivityByUserIdAsync(int userId);
        Task<List<UserActivityLog>> GetUserActivitiesAsync(int userId, int take = 20);
        Task<List<UserActivityLog>> GetUserActivitiesByDateRangeAsync(int userId, DateTime from, DateTime to);
        Task<int> GetTotalActivitiesCountAsync(int userId);
        Task DeleteOldLogsAsync(DateTime beforeDate);
    }
}
