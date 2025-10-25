using Application.DTOs.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Activity
{
    public class ArticleDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? FeaturedImage { get; set; }
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public AuthorDto? Author { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public string? AuthorName { get; set; }
    }

    public class ArticleListDto : BaseDto  // ADD INHERITANCE
    {


        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? FeaturedImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
        public List<string> Tags { get; set; } = new();
    }


    public class CreateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? FeaturedImage { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public List<int> ExistingTagIds { get; set; } = new();
        public List<string> NewTagNames { get; set; } = new();
    }

    public class UpdateArticleDto : CreateArticleDto
    {
        public int Id { get; set; }
    }

    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }

    public class CreateTagDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ArticleFilterDto
    {
        public string? SearchTerm { get; set; }
        public List<string>? Tags { get; set; }
        public ArticleStatus? Status { get; set; }
        public DateTime? PublishedFrom { get; set; }
        public DateTime? PublishedTo { get; set; }
        public string? SortBy { get; set; } = "PublishedAt";//
        public bool IsDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}