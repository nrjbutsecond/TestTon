using Application.DTOs.Advertisement;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureAdvertisementMappings()
        {
            CreateMap<AdvertisementModel, AdvertisementDto>()
                 .ForMember(dest => dest.AdType,
                     opt => opt.MapFrom(src => src.AdType.ToString()))
                 .ForMember(dest => dest.Position,
                     opt => opt.MapFrom(src => src.Position.ToString()))
                 .ForMember(dest => dest.Status,
                     opt => opt.MapFrom(src => src.Status.ToString()))
                 .ForMember(dest => dest.AdvertiserName,
                     opt => opt.MapFrom(src => src.Advertiser != null ? src.Advertiser.FullName : "Unknown"))
                 .ForMember(dest => dest.RemainingBudget,
                     opt => opt.MapFrom(src => src.RemainingBudget))
                 .ForMember(dest => dest.ClickThroughRate,
                     opt => opt.MapFrom(src => src.ClickThroughRate))
                 .ForMember(dest => dest.IsExpired,
                     opt => opt.MapFrom(src => src.IsExpired))
                 .ForMember(dest => dest.IsScheduledToStart,
                     opt => opt.MapFrom(src => src.IsScheduledToStart))
                 .ForMember(dest => dest.TargetAudience,
                     opt => opt.MapFrom(src => src.TargetAudience ?? "{}"));

            // AdvertisementModel to AdvertisementListDto
            CreateMap<AdvertisementModel, AdvertisementListDto>()
                .ForMember(dest => dest.AdType,
                    opt => opt.MapFrom(src => src.AdType.ToString()))
                .ForMember(dest => dest.Position,
                    opt => opt.MapFrom(src => src.Position.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ClickThroughRate,
                    opt => opt.MapFrom(src => src.ClickThroughRate));

            // CreateAdvertisementDto to AdvertisementModel
            CreateMap<CreateAdvertisementDto, AdvertisementModel>()
                .ForMember(dest => dest.AdType,
                    opt => opt.MapFrom(src => ParseAdType(src.AdType)))
                .ForMember(dest => dest.Position,
                    opt => opt.MapFrom(src => ParseAdPosition(src.Position)))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => AdStatus.Draft))
                .ForMember(dest => dest.ViewCount,
                    opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ClickCount,
                    opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.SpentAmount,
                    opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.TargetAudience,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.TargetAudience) ? "{}" : src.TargetAudience))
                .ForMember(dest => dest.Advertiser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.AdvertiserId,
                    opt => opt.Ignore()); // Set in service layer

            // UpdateAdvertisementDto to AdvertisementModel - Simplified
            CreateMap<UpdateAdvertisementDto, AdvertisementModel>()
                .ForAllMembers(opts => opts.Ignore()); // Manual mapping in service
        }

        // Helper methods for enum parsing
        private static AdType ParseAdType(string value)
        {
            if (Enum.TryParse<AdType>(value, true, out var result))
                return result;
            throw new ArgumentException($"Invalid AdType value: {value}");
        }

        private static AdPosition ParseAdPosition(string value)
        {
            if (Enum.TryParse<AdPosition>(value, true, out var result))
                return result;
            throw new ArgumentException($"Invalid AdPosition value: {value}");
        }

        private static AdStatus ParseAdStatus(string value)
        {
            if (Enum.TryParse<AdStatus>(value, true, out var result))
                return result;
            throw new ArgumentException($"Invalid AdStatus value: {value}");
        }
    }
}