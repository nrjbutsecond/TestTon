using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IArticleTagRepo : IRepo<ArticleTagModel>
    {
        Task<ArticleTagModel?> GetBySlugAsync(string slug);
        Task<IEnumerable<ArticleTagModel>> GetPopularTagsAsync(int count);
        Task<IEnumerable<ArticleTagModel>> GetTagsWithArticleCountAsync();
        Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null);
    }
}
