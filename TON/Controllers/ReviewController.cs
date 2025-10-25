using Application.DTOs.Review;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserId();
            var review = await _reviewService.CreateReviewAsync(userId, dto);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            // Implementation for single review
            return Ok();
        }

        [HttpGet("merchandise/{merchandiseId}")]
        public async Task<IActionResult> GetMerchandiseReviews(
            int merchandiseId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.Identity?.IsAuthenticated == true ? GetUserId() : (int?)null;
            var reviews = await _reviewService.GetMerchandiseReviewsAsync(merchandiseId, userId, pageNumber, pageSize);
            return Ok(reviews);
        }

        [HttpGet("merchandise/{merchandiseId}/stats")]
        public async Task<IActionResult> GetReviewStats(int merchandiseId)
        {
            var stats = await _reviewService.GetReviewStatsAsync(merchandiseId);
            return Ok(stats);
        }

        [HttpGet("merchandise/{merchandiseId}/user")]
        [Authorize]
        public async Task<IActionResult> GetUserReview(int merchandiseId)
        {
            var userId = GetUserId();
            var review = await _reviewService.GetUserReviewAsync(userId, merchandiseId);
            return Ok(review);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
        {
            var userId = GetUserId();
            var review = await _reviewService.UpdateReviewAsync(userId, id, dto);
            return Ok(review);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = GetUserId();
            await _reviewService.DeleteReviewAsync(userId, id);
            return NoContent();
        }

        [HttpPost("{id}/responses")]
        [Authorize]
        public async Task<IActionResult> AddResponse(int id, [FromBody] CreateReviewResponseDto dto)
        {
            var userId = GetUserId();
            var response = await _reviewService.AddResponseAsync(userId, id, dto);
            return Ok(response);
        }

        [HttpPost("{id}/vote")]
        [Authorize]
        public async Task<IActionResult> VoteReview(int id, [FromBody] ReviewVoteDto dto)
        {
            var userId = GetUserId();
            await _reviewService.VoteReviewAsync(userId, id, dto);
            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        }
    }
}