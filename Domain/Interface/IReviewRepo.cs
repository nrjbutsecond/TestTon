using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IReviewRepo :IRepo<ReviewModel>
    {
        Task<ReviewModel?> GetReviewWithDetailsAsync(int reviewId);
        Task<IEnumerable<ReviewModel>> GetMerchandiseReviewsAsync(int merchandiseId, int pageNumber = 1, int pageSize = 10);
        Task<bool> HasUserReviewedMerchandiseAsync(int userId, int merchandiseId);
        Task<ReviewModel?> GetUserReviewForMerchandiseAsync(int userId, int merchandiseId);
        Task<bool> CanUserReviewMerchandiseAsync(int userId, int merchandiseId);
        Task<ReviewStats> GetMerchandiseReviewStatsAsync(int merchandiseId);
    }
}
