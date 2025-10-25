using Application.DTOs.Review;
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
        private void ConfigureReviewMappings()
        {
            CreateMap<ReviewModel, ReviewDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => DeserializeImages(src.Images ?? "[]")))
                .ForMember(dest => dest.UserVote,
                    opt => opt.Ignore()); // Set in service

            CreateMap<CreateReviewDto, ReviewModel>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => SerializeImages(src.Images)))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItemId, opt => opt.Ignore());

            CreateMap<UpdateReviewDto, ReviewModel>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => SerializeImages(src.Images)))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.MerchandiseId, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.Ignore());

            CreateMap<ReviewResponseModel, ReviewResponseDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}