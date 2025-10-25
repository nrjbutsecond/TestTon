using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class ReviewRepo: Repo<ReviewModel>, IReviewRepo
    {
        public ReviewRepo(AppDbContext context) : base(context) { }

        public async Task<ReviewModel?> GetReviewWithDetailsAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Merchandise)
                .Include(r => r.Responses)
                    .ThenInclude(rr => rr.User)
                .Include(r => r.Votes)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<IEnumerable<ReviewModel>> GetMerchandiseReviewsAsync(int merchandiseId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Responses)
                    .ThenInclude(rr => rr.User)
                .Where(r => r.MerchandiseId == merchandiseId)
                .OrderByDescending(r => r.IsVerifiedPurchase)
                .ThenByDescending(r => r.HelpfulCount)
                .ThenByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> HasUserReviewedMerchandiseAsync(int userId, int merchandiseId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.MerchandiseId == merchandiseId);
        }

        public async Task<ReviewModel?> GetUserReviewForMerchandiseAsync(int userId, int merchandiseId)
        {
            return await _context.Reviews
                .Include(r => r.Responses)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MerchandiseId == merchandiseId);
        }

        public async Task<bool> CanUserReviewMerchandiseAsync(int userId, int merchandiseId)
        {
            // Check if user has purchased this merchandise
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .AnyAsync(oi =>
                    oi.MerchandiseId == merchandiseId &&
                    oi.Order.UserId == userId &&
                    oi.Order.Status == OrderStatus.Delivered);
        }

        public async Task<ReviewStats> GetMerchandiseReviewStatsAsync(int merchandiseId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.MerchandiseId == merchandiseId)
                .ToListAsync();

            if (!reviews.Any())
                return new ReviewStats();

            return new ReviewStats
            {
                TotalReviews = reviews.Count,
                AverageRating = Math.Round(reviews.Average(r => r.Rating), 1),
                FiveStarCount = reviews.Count(r => r.Rating == 5),
                FourStarCount = reviews.Count(r => r.Rating == 4),
                ThreeStarCount = reviews.Count(r => r.Rating == 3),
                TwoStarCount = reviews.Count(r => r.Rating == 2),
                OneStarCount = reviews.Count(r => r.Rating == 1),
                VerifiedPurchaseCount = reviews.Count(r => r.IsVerifiedPurchase)
            };
        }
    }


}

