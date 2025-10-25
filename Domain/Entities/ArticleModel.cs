using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ArticleModel :BaseEntity
    {
        public int Author { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? FeaturedImage { get; set; }
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; } = 0;

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        // Navigation
        public virtual ICollection<ArticleTagModel> Tags { get; set; } = new List<ArticleTagModel>();

        public virtual UserModel AuthorUser { get; set; }
    }

    public enum ArticleStatus
    {
        Draft,
        Published,
        Archived
    }

    
}