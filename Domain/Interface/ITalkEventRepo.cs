using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ITalkEventRepo : IRepo<TalkEventModel>
    {
        Task<IEnumerable<TalkEventModel>> GetUpcomingEventsAsync(int limit = 10);
        Task<IEnumerable<TalkEventModel>> GetEventsByOrganizerAsync(int organizerId);
        Task<IEnumerable<TalkEventModel>> GetPartneredEventsAsync();
        Task<(IEnumerable<TalkEventModel> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TalkEventModel, bool>>? filter = null,
            string? orderBy = null);
        Task<TalkEventModel?> GetEventWithTicketsAsync(int id);

        Task<TalkEventModel?> GetDeletedByIdAsync(int id);
        Task<IEnumerable<TalkEventModel>> GetDeletedEventsAsync(
            Expression<Func<TalkEventModel, bool>>? filter = null,
            string? orderBy = null);
        Task<int> CountDeletedEventsAsync(Expression<Func<TalkEventModel, bool>>? filter = null);
    }
}
