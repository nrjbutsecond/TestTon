using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IArticleRepo : IRepo<ArticleModel>
    {
        Task<IEnumerable<ArticleModel>> GetPublishedArticlesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<ArticleModel>> GetArticlesByAuthorAsync(int authorId);
        Task<IEnumerable<ArticleModel>> GetArticlesByTagAsync(string tag);
        Task<IEnumerable<ArticleModel>> SearchArticlesAsync(string searchTerm);
        Task<int> IncrementViewCountAsync(int articleId);
        Task<IEnumerable<ArticleModel>> GetPopularArticlesAsync(int count);
        Task<IEnumerable<ArticleModel>> GetRelatedArticlesAsync(int articleId, int count);
    }
}
