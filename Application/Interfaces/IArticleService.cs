using Application.DTOs.Activity;
using Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IArticleService
    {
        Task<PagedResult<ArticleListDto>> GetPublishedArticlesAsync(int pageNumber, int pageSize);
        Task<ArticleDto?> GetArticleByIdAsync(int id);
        Task<ArticleDto?> GetArticleBySlugAsync(string slug);
        Task<PagedResult<ArticleListDto>> GetArticlesAsync(ArticleFilterDto filter);
        Task<IEnumerable<ArticleListDto>> GetArticlesByTagAsync(string tagSlug);
        Task<IEnumerable<ArticleListDto>> SearchArticlesAsync(string searchTerm);
        Task<IEnumerable<ArticleListDto>> GetPopularArticlesAsync(int count);
        Task<IEnumerable<ArticleListDto>> GetRelatedArticlesAsync(int articleId, int count);
        Task<ArticleDto> CreateArticleAsync(CreateArticleDto dto, int userId);
        Task<ArticleDto?> UpdateArticleAsync(UpdateArticleDto dto, int userId);
        Task<bool> DeleteArticleAsync(int id, int userId);
        Task<bool> PublishArticleAsync(int id, int userId);
        Task<bool> ArchiveArticleAsync(int id, int userId);
        Task<int> IncrementViewCountAsync(int id);

        // Tag operations
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<IEnumerable<TagDto>> GetPopularTagsAsync(int count);
        Task<TagDto> CreateTagAsync(CreateTagDto dto);
        Task<bool> DeleteTagAsync(int id);
    }
}
