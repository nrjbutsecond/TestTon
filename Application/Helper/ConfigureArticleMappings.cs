using Application.DTOs.Activity;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureArticleMappings()
        {
            // Article mappings
            CreateMap<ArticleModel, ArticleDto>()
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.Author,
                    opt => opt.MapFrom(src => src.AuthorUser != null ? new AuthorDto
                    {
                        Id = src.AuthorUser.Id,
                        Name = src.AuthorUser.FullName,
                        Email = src.AuthorUser.Email
                    } : null))
                .ForMember(dest => dest.AuthorName,
                    opt => opt.MapFrom(src => src.AuthorUser != null ? src.AuthorUser.FullName : "Unknown"));

            CreateMap<ArticleModel, ArticleListDto>()
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));

            CreateMap<CreateArticleDto, ArticleModel>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorUser, opt => opt.Ignore())
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => 0));

            CreateMap<UpdateArticleDto, ArticleModel>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorUser, opt => opt.Ignore())
                .ForMember(dest => dest.ViewCount, opt => opt.Ignore());

            // ArticleTag mappings
            CreateMap<ArticleTagModel, TagDto>();
            CreateMap<CreateTagDto, ArticleTagModel>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.Articles, opt => opt.Ignore());
        }
    }
}
