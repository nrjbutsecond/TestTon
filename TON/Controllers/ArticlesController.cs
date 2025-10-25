using Application.DTOs.Activity;
using Application.DTOs.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IArticleService articleService, ILogger<ArticlesController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }

        // ===== ARTICLE ENDPOINTS =====

        /// <summary>
        /// Get published articles with pagination
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ArticleListDto>>> GetArticles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var articles = await _articleService.GetPublishedArticlesAsync(pageNumber, pageSize);
            return Ok(articles);
        }

        /// <summary>
        /// Advanced search with filters
        /// </summary>
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ArticleListDto>>> SearchArticles(
            [FromBody] ArticleFilterDto filter)
        {
            var articles = await _articleService.GetArticlesAsync(filter);
            return Ok(articles);
        }

        /// <summary>
        /// Quick search by keyword
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ArticleListDto>>> SearchArticles(
            [FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty");

            var articles = await _articleService.SearchArticlesAsync(query);
            return Ok(articles);
        }

        /// <summary>
        /// Get article by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ArticleDto>> GetArticle(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            // Increment view count
            await _articleService.IncrementViewCountAsync(id);

            return Ok(article);
        }

        /// <summary>
        /// Get article by slug
        /// </summary>
        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<ArticleDto>> GetArticleBySlug(string slug)
        {
            var article = await _articleService.GetArticleBySlugAsync(slug);
            if (article == null)
                return NotFound();

            // Increment view count
            await _articleService.IncrementViewCountAsync(article.Id);

            return Ok(article);
        }

        /// <summary>
        /// Get articles by tag
        /// </summary>
        [HttpGet("tag/{tagSlug}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ArticleListDto>>> GetArticlesByTag(string tagSlug)
        {
            var articles = await _articleService.GetArticlesByTagAsync(tagSlug);
            return Ok(articles);
        }

        /// <summary>
        /// Get popular articles
        /// </summary>
        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ArticleListDto>>> GetPopularArticles(
            [FromQuery] int count = 5)
        {
            var articles = await _articleService.GetPopularArticlesAsync(count);
            return Ok(articles);
        }

        /// <summary>
        /// Get related articles
        /// </summary>
        [HttpGet("{id:int}/related")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ArticleListDto>>> GetRelatedArticles(
            int id,
            [FromQuery] int count = 5)
        {
            var articles = await _articleService.GetRelatedArticlesAsync(id, count);
            return Ok(articles);
        }

        /// <summary>
        /// Create new article
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto dto)
        {
            var userId = GetCurrentUserId();
            var article = await _articleService.CreateArticleAsync(dto, userId);

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        /// <summary>
        /// Update article
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult<ArticleDto>> UpdateArticle(int id, UpdateArticleDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var userId = GetCurrentUserId();
            var article = await _articleService.UpdateArticleAsync(dto, userId);

            if (article == null)
                return NotFound();

            return Ok(article);
        }

        /// <summary>
        /// Delete article
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult> DeleteArticle(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _articleService.DeleteArticleAsync(id, userId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Publish article
        /// </summary>
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult> PublishArticle(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _articleService.PublishArticleAsync(id, userId);

            if (!result)
                return NotFound();

            return Ok(new { message = "Article published successfully" });
        }

        /// <summary>
        /// Archive article
        /// </summary>
        [HttpPost("{id}/archive")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult> ArchiveArticle(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _articleService.ArchiveArticleAsync(id, userId);

            if (!result)
                return NotFound();

            return Ok(new { message = "Article archived successfully" });
        }

        // ===== TAG ENDPOINTS =====

        /// <summary>
        /// Get all tags
        /// </summary>
        [HttpGet("tags")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
        {
            var tags = await _articleService.GetAllTagsAsync();
            return Ok(tags);
        }

        /// <summary>
        /// Get popular tags
        /// </summary>
        [HttpGet("tags/popular")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetPopularTags(
            [FromQuery] int count = 10)
        {
            var tags = await _articleService.GetPopularTagsAsync(count);
            return Ok(tags);
        }

        /// <summary>
        /// Create new tag
        /// </summary>
        [HttpPost("tags")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto)
        {
            try
            {
                var tag = await _articleService.CreateTagAsync(dto);
                return CreatedAtAction(nameof(GetAllTags), new { id = tag.Id }, tag);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete tag
        /// </summary>
        [HttpDelete("tags/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            var result = await _articleService.DeleteTagAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // Helper method
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return 0;
        }
    }
}