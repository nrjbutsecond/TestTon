using Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task<ReviewDto> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto);
        Task<bool> DeleteReviewAsync(int userId, int reviewId);
        Task<IEnumerable<ReviewDto>> GetMerchandiseReviewsAsync(int merchandiseId, int? currentUserId = null, int pageNumber = 1, int pageSize = 10);
        Task<ReviewDto?> GetUserReviewAsync(int userId, int merchandiseId);
        Task<ReviewStatsDto> GetReviewStatsAsync(int merchandiseId);
        Task<ReviewResponseDto> AddResponseAsync(int userId, int reviewId, CreateReviewResponseDto dto);
        Task<bool> VoteReviewAsync(int userId, int reviewId, ReviewVoteDto dto);
    }
}
