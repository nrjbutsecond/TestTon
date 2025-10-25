using Application.DTOs.Review;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ReviewService :IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            // Check if user already reviewed
            var existingReview = await _unitOfWork.Reviews
                .HasUserReviewedMerchandiseAsync(userId, dto.MerchandiseId);
            if (existingReview)
                throw new InvalidOperationException("You have already reviewed this product");

            // Check if user can review (has purchased)
            var canReview = await _unitOfWork.Reviews
                .CanUserReviewMerchandiseAsync(userId, dto.MerchandiseId);

            // Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5");

            var review = _mapper.Map<ReviewModel>(dto);
            review.UserId = userId;
            review.IsVerifiedPurchase = canReview;
            review.CreatedAt = DateTime.UtcNow;

            // Find the order item if verified purchase
            if (canReview)
            {
                var orderItem = await _unitOfWork.OrderItems
                    .FindAsync(oi => oi.Order.UserId == userId &&
                                    oi.MerchandiseId == dto.MerchandiseId &&
                                    oi.Order.Status == OrderStatus.Delivered);
                review.OrderItemId = orderItem.FirstOrDefault()?.Id;
            }

            await _unitOfWork.Reviews.AddAsync(review);

            // Update merchandise average rating
            await UpdateMerchandiseRating(dto.MerchandiseId);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                throw new InvalidOperationException("Review not found or unauthorized");

            // Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5");

            _mapper.Map(dto, review);
            review.UpdatedAt = DateTime.UtcNow;

            // Update merchandise average rating
            await UpdateMerchandiseRating(review.MerchandiseId);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                throw new InvalidOperationException("Review not found or unauthorized");

            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;

            // Update merchandise average rating
            await UpdateMerchandiseRating(review.MerchandiseId);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReviewDto>> GetMerchandiseReviewsAsync(int merchandiseId, int? currentUserId = null, int pageNumber = 1, int pageSize = 10)
        {
            var reviews = await _unitOfWork.Reviews
                .GetMerchandiseReviewsAsync(merchandiseId, pageNumber, pageSize);

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            // Add user vote status if user is logged in
            if (currentUserId.HasValue)
            {
                var userVotes = await _unitOfWork.ReviewVotes
                    .FindAsync(v => v.UserId == currentUserId.Value &&
                                   reviews.Select(r => r.Id).Contains(v.ReviewId));

                foreach (var dto in reviewDtos)
                {
                    var vote = userVotes.FirstOrDefault(v => v.ReviewId == dto.Id);
                    dto.UserVote = vote?.IsHelpful;
                }
            }

            return reviewDtos;
        }

        public async Task<ReviewDto?> GetUserReviewAsync(int userId, int merchandiseId)
        {
            var review = await _unitOfWork.Reviews
                .GetUserReviewForMerchandiseAsync(userId, merchandiseId);

            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewStatsDto> GetReviewStatsAsync(int merchandiseId)
        {
            var stats = await _unitOfWork.Reviews.GetMerchandiseReviewStatsAsync(merchandiseId);

            return new ReviewStatsDto
            {
                TotalReviews = stats.TotalReviews,
                AverageRating = stats.AverageRating,
                RatingDistribution = new Dictionary<int, int>
                {
                    { 5, stats.FiveStarCount },
                    { 4, stats.FourStarCount },
                    { 3, stats.ThreeStarCount },
                    { 2, stats.TwoStarCount },
                    { 1, stats.OneStarCount }
                },
                VerifiedPurchaseCount = stats.VerifiedPurchaseCount,
                VerifiedPurchasePercentage = stats.TotalReviews > 0
                    ? Math.Round((double)stats.VerifiedPurchaseCount / stats.TotalReviews * 100, 1)
                    : 0
            };
        }

        public async Task<ReviewResponseDto> AddResponseAsync(int userId, int reviewId, CreateReviewResponseDto dto)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithDetailsAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found");

            // Check if user is seller
            var isSellerResponse = review.Merchandise.SellerId == userId;

            var response = new ReviewResponseModel
            {
                ReviewId = reviewId,
                UserId = userId,
                Content = dto.Content,
                IsSellerResponse = isSellerResponse,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ReviewResponses.AddAsync(response);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewResponseDto>(response);
        }

        public async Task<bool> VoteReviewAsync(int userId, int reviewId, ReviewVoteDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found");

            // User cannot vote on their own review
            if (review.UserId == userId)
                throw new InvalidOperationException("Cannot vote on your own review");

            // Check existing vote
            var existingVote = await _unitOfWork.ReviewVotes
                .FindAsync(v => v.UserId == userId && v.ReviewId == reviewId);

            var vote = existingVote.FirstOrDefault();

            if (vote != null)
            {
                // Update existing vote
                if (vote.IsHelpful != dto.IsHelpful)
                {
                    // Update counts
                    if (dto.IsHelpful)
                    {
                        review.HelpfulCount++;
                        review.UnhelpfulCount--;
                    }
                    else
                    {
                        review.HelpfulCount--;
                        review.UnhelpfulCount++;
                    }

                    vote.IsHelpful = dto.IsHelpful;
                    vote.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                // Create new vote
                vote = new ReviewVoteModel
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    IsHelpful = dto.IsHelpful,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ReviewVotes.AddAsync(vote);

                // Update counts
                if (dto.IsHelpful)
                    review.HelpfulCount++;
                else
                    review.UnhelpfulCount++;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task UpdateMerchandiseRating(int merchandiseId)
        {
            var stats = await _unitOfWork.Reviews.GetMerchandiseReviewStatsAsync(merchandiseId);
            var merchandise = await _unitOfWork.Merchandises.GetByIdAsync(merchandiseId);

            if (merchandise != null)
            {
                // Store average rating in merchandise (add this field if needed)
                 //merchandise.AverageRating = stats.AverageRating;
                 //merchandise.ReviewCount = stats.TotalReviews;
            }
        }
    }
}