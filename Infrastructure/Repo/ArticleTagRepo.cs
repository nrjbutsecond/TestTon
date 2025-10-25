using Domain.Entities;
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
    public class ArticleTagRepo :  Repo<ArticleTagModel>, IArticleTagRepo
    {
        public ArticleTagRepo(AppDbContext context) : base(context) { }

        public async Task<ArticleTagModel?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Slug == slug && !t.IsDeleted);
        }

        public async Task<IEnumerable<ArticleTagModel>> GetPopularTagsAsync(int count)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Articles.Count(a =>
                    a.Status == ArticleStatus.Published && !a.IsDeleted))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleTagModel>> GetTagsWithArticleCountAsync()
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .Select(t => new
                {
                    Tag = t,
                    ArticleCount = t.Articles.Count(a =>
                        a.Status == ArticleStatus.Published && !a.IsDeleted)
                })
                .Where(x => x.ArticleCount > 0)
                .OrderByDescending(x => x.ArticleCount)
                .Select(x => x.Tag)
                .ToListAsync();
        }

        public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _dbSet.Where(t => t.Slug == slug && !t.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}