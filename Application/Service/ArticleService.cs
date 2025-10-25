using Application.DTOs.Activity;
using Application.DTOs.Common;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Helper;


namespace Application.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ArticleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // Article operations
        public async Task<PagedResult<ArticleListDto>> GetPublishedArticlesAsync(int pageNumber, int pageSize)
        {
            var articles = await _unitOfWork.Articles.GetPublishedArticlesAsync(pageNumber, pageSize);
            var totalCount = await _unitOfWork.Articles.CountAsync(
                a => a.Status == ArticleStatus.Published && !a.IsDeleted);

            return new PagedResult<ArticleListDto>
            {
                 Items = _mapper.Map<IEnumerable<ArticleListDto>>(articles).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ArticleDto?> GetArticleByIdAsync(int id)
        {
            var article = await _unitOfWork.Repo<ArticleModel>()
                .GetQueryable()
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (article == null || article.Status != ArticleStatus.Published)
                return null;

            var dto = _mapper.Map<ArticleDto>(article);
            dto.AuthorName = await GetUserNameAsync(article.Author);
            return dto;
        }

        public async Task<ArticleDto?> GetArticleBySlugAsync(string slug)
        {
            var article = await _unitOfWork.Repo<ArticleModel>()
                .GetQueryable()
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Slug == slug &&
                    a.Status == ArticleStatus.Published &&
                    !a.IsDeleted);

            if (article == null)
                return null;

            var dto = _mapper.Map<ArticleDto>(article);
            dto.AuthorName = await GetUserNameAsync(article.Author);
            return dto;
        }

        public async Task<PagedResult<ArticleListDto>> GetArticlesAsync(ArticleFilterDto filter)
        {
            var query = _unitOfWork.Repo<ArticleModel>()
                .GetQueryable()
                .Include(a => a.Tags)
                .Where(a => !a.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(searchTerm) ||
                                        a.Summary!.ToLower().Contains(searchTerm));
            }

            if (filter.Status.HasValue)
            {
                
                    query = query.Where(a => a.Status == filter.Status.Value);
                
            }

            if (filter.Tags != null && filter.Tags.Any())
            {
                query = query.Where(a => a.Tags.Any(t => filter.Tags.Contains(t.Slug)));
            }

            if (filter.PublishedFrom.HasValue)
            {
                query = query.Where(a => a.PublishedAt >= filter.PublishedFrom.Value);
            }

            if (filter.PublishedTo.HasValue)
            {
                query = query.Where(a => a.PublishedAt <= filter.PublishedTo.Value);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "viewcount" => filter.IsDescending
                    ? query.OrderByDescending(a => a.ViewCount)
                    : query.OrderBy(a => a.ViewCount),
                "title" => filter.IsDescending
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title),
                _ => filter.IsDescending
                    ? query.OrderByDescending(a => a.PublishedAt)
                    : query.OrderBy(a => a.PublishedAt)
            };

            var totalCount = await query.CountAsync();
            var articles = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<ArticleListDto>
            {
                Items = _mapper.Map<IEnumerable<ArticleListDto>>(articles).ToList(),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<ArticleListDto>> GetArticlesByTagAsync(string tagSlug)
        {
            var articles = await _unitOfWork.Articles.GetArticlesByTagAsync(tagSlug);
            return _mapper.Map<IEnumerable<ArticleListDto>>(articles);
        }

        public async Task<IEnumerable<ArticleListDto>> SearchArticlesAsync(string searchTerm)
        {
            var articles = await _unitOfWork.Articles.SearchArticlesAsync(searchTerm);
            return _mapper.Map<IEnumerable<ArticleListDto>>(articles);
        }

        public async Task<IEnumerable<ArticleListDto>> GetPopularArticlesAsync(int count)
        {
            var articles = await _unitOfWork.Articles.GetPopularArticlesAsync(count);
            return _mapper.Map<IEnumerable<ArticleListDto>>(articles);
        }

        public async Task<IEnumerable<ArticleListDto>> GetRelatedArticlesAsync(int articleId, int count)
        {
            var articles = await _unitOfWork.Articles.GetRelatedArticlesAsync(articleId, count);
            return _mapper.Map<IEnumerable<ArticleListDto>>(articles);
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto dto, int userId)
        {
            var article = _mapper.Map<ArticleModel>(dto);

            // Generate slug
            var baseSlug = SlugHelper.GenerateSlug(dto.Title);
            article.Slug = await EnsureUniqueSlugAsync(baseSlug);

            // Set status
            article.Status = dto.IsPublished ? ArticleStatus.Published : ArticleStatus.Draft;
            if (dto.IsPublished)
            {
                article.PublishedAt = DateTime.UtcNow;
            }

            // Handle tags
            var tags = new List<ArticleTagModel>();

            // Add existing tags
            if (dto.ExistingTagIds.Any())
            {
                var existingTags = await _unitOfWork.Repo<ArticleTagModel>()
                    .GetQueryable()
                    .Where(t => dto.ExistingTagIds.Contains(t.Id))
                    .ToListAsync();
                tags.AddRange(existingTags);
            }

            // Create new tags
            foreach (var tagName in dto.NewTagNames)
            {
                var tagSlug = SlugHelper.GenerateSlug(tagName);
                var existingTag = await _unitOfWork.Repo<ArticleTagModel>()
                    .GetQueryable()
                    .FirstOrDefaultAsync(t => t.Slug == tagSlug);

                if (existingTag != null)
                {
                    tags.Add(existingTag);
                }
                else
                {
                    var newTag = new ArticleTagModel
                    {
                        Name = tagName,
                        Slug = tagSlug
                    };
                    await _unitOfWork.Repo<ArticleTagModel>().AddAsync(newTag);
                    tags.Add(newTag);
                }
            }

            article.Tags = tags;
            article.Author = userId;

            await _unitOfWork.Articles.AddAsync(article);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Article created with ID: {ArticleId}", article.Id);

            var result = _mapper.Map<ArticleDto>(article);
            result.AuthorName = await GetUserNameAsync(userId);
            return result;
        }

        public async Task<ArticleDto?> UpdateArticleAsync(UpdateArticleDto dto, int userId)
        {
            var article = await _unitOfWork.Repo<ArticleModel>()
                .GetQueryable()
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == dto.Id && !a.IsDeleted);

            if (article == null)
                return null;

            // Update basic properties
            article.Title = dto.Title;
            article.Content = dto.Content;
            article.Summary = dto.Summary;
            article.FeaturedImage = dto.FeaturedImage;
            article.MetaTitle = dto.MetaTitle;
            article.MetaDescription = dto.MetaDescription;
            article.UpdatedBy = GetUserNameAsync(userId).ToString();
            article.UpdatedAt = DateTime.UtcNow;

            // Update slug if title changed
            if (article.Title != dto.Title)
            {
                var baseSlug = SlugHelper.GenerateSlug(dto.Title);
                article.Slug = await EnsureUniqueSlugAsync(baseSlug, article.Id);
            }

            // Update status
            var wasPublished = article.Status == ArticleStatus.Published;
            article.Status = dto.IsPublished ? ArticleStatus.Published : ArticleStatus.Draft;

            if (dto.IsPublished && !wasPublished)
            {
                article.PublishedAt = DateTime.UtcNow;
            }

            // Update tags
            article.Tags.Clear();

            // Add existing tags
            if (dto.ExistingTagIds.Any())
            {
                var existingTags = await _unitOfWork.Repo<ArticleTagModel>()
                    .GetQueryable()
                    .Where(t => dto.ExistingTagIds.Contains(t.Id))
                    .ToListAsync();

                foreach (var tag in existingTags)
                {
                    article.Tags.Add(tag);
                }
            }

            // Create new tags
            foreach (var tagName in dto.NewTagNames)
            {
                var tagSlug = SlugHelper.GenerateSlug(tagName);
                var existingTag = await _unitOfWork.Repo<ArticleTagModel>()
                    .GetQueryable()
                    .FirstOrDefaultAsync(t => t.Slug == tagSlug);

                if (existingTag != null)
                {
                    article.Tags.Add(existingTag);
                }
                else
                {
                    var newTag = new ArticleTagModel
                    {
                        Name = tagName,
                        Slug = tagSlug
                    };
                    await _unitOfWork.Repo<ArticleTagModel>().AddAsync(newTag);
                    article.Tags.Add(newTag);
                }
            }

            _unitOfWork.Articles.Update(article);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ArticleDto>(article);
            result.AuthorName = await GetUserNameAsync(userId);
            return result;
        }

        public async Task<bool> DeleteArticleAsync(int id, int userId)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article == null)
                return false;

            _unitOfWork.Articles.Remove(article);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Article deleted with ID: {ArticleId}", id);
            return true;
        }

        public async Task<bool> PublishArticleAsync(int id, int userId)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article == null)
                return false;

            article.Status = ArticleStatus.Published;
            article.PublishedAt = DateTime.UtcNow;
            article.UpdatedBy = GetUserNameAsync(userId).ToString();
            article.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Articles.Update(article);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ArchiveArticleAsync(int id, int userId)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article == null)
                return false;

            article.Status = ArticleStatus.Archived;
            article.UpdatedBy = GetUserNameAsync(userId).ToString();
            article.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Articles.Update(article);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> IncrementViewCountAsync(int id)
        {
            return await _unitOfWork.Articles.IncrementViewCountAsync(id);
        }

        // Tag operations
        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.Repo<ArticleTagModel>()
                .GetQueryable()
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TagDto>>(tags);
        }

        public async Task<IEnumerable<TagDto>> GetPopularTagsAsync(int count)
        {
            var tags = await _unitOfWork.Repo<ArticleTagModel>()
                .GetQueryable()
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Articles.Count(a => a.Status == ArticleStatus.Published))
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TagDto>>(tags);
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
        {
            var slug = SlugHelper.GenerateSlug(dto.Name);

            // Check if tag already exists
            var existingTag = await _unitOfWork.Repo<ArticleTagModel>()
                .GetQueryable()
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (existingTag != null)
            {
                throw new ConflictException($"Tag with slug '{slug}' already exists");
            }

            var tag = new ArticleTagModel
            {
                Name = dto.Name,
                Slug = slug
            };

            await _unitOfWork.Repo<ArticleTagModel>().AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TagDto>(tag);
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _unitOfWork.Repo<ArticleTagModel>().GetByIdAsync(id);
            if (tag == null)
                return false;

            _unitOfWork.Repo<ArticleTagModel>().Remove(tag);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Helper methods
        private async Task<string> EnsureUniqueSlugAsync(string baseSlug, int? excludeId = null)
        {
            return SlugHelper.EnsureUniqueSlug(baseSlug, slug =>
            {
                var query = _unitOfWork.Repo<ArticleModel>()
                    .GetQueryable()
                    .Where(a => a.Slug == slug);

                if (excludeId.HasValue)
                {
                    query = query.Where(a => a.Id != excludeId.Value);
                }

                return query.Any();
            });
        }

        private async Task<string> GetUserNameAsync(int? userId)
        {
            if (!userId.HasValue || userId.Value <= 0)
                return "Unknown";

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
            return user?.FullName ?? "Unknown";
        }
    }
}