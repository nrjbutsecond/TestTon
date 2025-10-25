using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Interface;
namespace Infrastructure.Repo
{
    public class ArticleRepo : Repo<ArticleModel>, IArticleRepo
    {
        public ArticleRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ArticleModel>> GetPublishedArticlesAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(a => a.Status == ArticleStatus.Published && !a.IsDeleted)
                .OrderByDescending(a => a.PublishedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByAuthorAsync(int authorId)
        {
            // Assuming CreatedBy contains user identifier
            return await _dbSet
                .Where(a => a.CreatedBy == authorId.ToString() && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByTagAsync(string tag)
        {
            return await _dbSet
                .Where(a => a.Tags.Any(t => t.Slug == tag) &&
                       a.Status == ArticleStatus.Published &&
                       !a.IsDeleted)
                .OrderByDescending(a => a.PublishedAt)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleModel>> SearchArticlesAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _dbSet
                .Where(a => !a.IsDeleted &&
                       a.Status == ArticleStatus.Published &&
                       (a.Title.ToLower().Contains(lowerSearchTerm) ||
                        a.Summary!.ToLower().Contains(lowerSearchTerm) ||
                        a.Tags.Any(t => t.Name.ToLower().Contains(lowerSearchTerm))))
                .OrderByDescending(a => a.PublishedAt)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<int> IncrementViewCountAsync(int articleId)
        {
            var article = await _dbSet.FindAsync(articleId);
            if (article != null && !article.IsDeleted)
            {
                article.ViewCount++;
                await _context.SaveChangesAsync();
                return article.ViewCount;
            }
            return 0;
        }

        public async Task<IEnumerable<ArticleModel>> GetPopularArticlesAsync(int count)
        {
            return await _dbSet
                .Where(a => a.Status == ArticleStatus.Published && !a.IsDeleted)
                .OrderByDescending(a => a.ViewCount)
                .Take(count)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleModel>> GetRelatedArticlesAsync(int articleId, int count)
        {
            var article = await _dbSet
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == articleId);

            if (article == null) return new List<ArticleModel>();

            var tagIds = article.Tags.Select(t => t.Id).ToList();

            return await _dbSet
                .Where(a => a.Id != articleId &&
                       a.Status == ArticleStatus.Published &&
                       !a.IsDeleted &&
                       a.Tags.Any(t => tagIds.Contains(t.Id)))
                .OrderByDescending(a => a.Tags.Count(t => tagIds.Contains(t.Id)))
                .ThenByDescending(a => a.PublishedAt)
                .Take(count)
                .Include(a => a.Tags)
                .ToListAsync();
        }
    }
}