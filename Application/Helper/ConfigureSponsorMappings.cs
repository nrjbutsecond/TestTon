using Application.DTOs.Sponsor;
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
        private void ConfigureSponsorMappings()
        {
            // Entity to DTOs
            CreateMap<SponsorModel, SponsorDto>()
                .ForMember(dest => dest.IsContractActive,
                    opt => opt.MapFrom(src => src.IsContractActive()))
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => DeserializeBenefits(src.Benefits)));

            CreateMap<SponsorModel, PublicSponsorDto>()
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => DeserializeBenefits(src.Benefits)));

            // DTOs to Entity
            CreateMap<CreateSponsorDto, SponsorModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => SerializeBenefits(src.Benefits)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateSponsorDto, SponsorModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => SerializeBenefits(src.Benefits)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // For updating existing entity
            CreateMap<UpdateSponsorDto, SponsorModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => SerializeBenefits(src.Benefits)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

        // Helper methods for JSON serialization
        private static string? SerializeBenefits(List<string>? benefits)
        {
            return benefits != null && benefits.Count > 0
                ? JsonSerializer.Serialize(benefits)
                : null;
        }

        private static List<string>? DeserializeBenefits(string? benefitsJson)
        {
            if (string.IsNullOrWhiteSpace(benefitsJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<string>>(benefitsJson);
            }
            catch
            {
                return null;
            }
        }
    }
}