using Application.DTOs.Merchandise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.MerchandiseEntity;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureMerchandiseMappings()
        {
            // Merchandise mappings
            CreateMap<Merchandise, MerchandiseDto>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => DeserializeImages(src.Images)))
                .ForMember(dest => dest.AvailableStock,
                    opt => opt.MapFrom(src => src.StockQuantity - src.ReservedQuantity))
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FullName : "Official TON"));

            CreateMap<Merchandise, MerchandiseDetailDto>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => DeserializeImages(src.Images)))
                .ForMember(dest => dest.AvailableStock,
                    opt => opt.MapFrom(src => src.StockQuantity - src.ReservedQuantity))
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FullName : "Official TON"))
                .ForMember(dest => dest.Variants,
                    opt => opt.MapFrom(src => src.Variants.Where(v => !v.IsDeleted))) // ✅ FIX: Filter deleted
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore()) // ✅ ADD
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore());  // ✅ ADD

            CreateMap<Merchandise, MerchandiseListDto>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => DeserializeImages(src.Images).Take(1).ToList()))
                .ForMember(dest => dest.AvailableStock,
                    opt => opt.MapFrom(src => src.StockQuantity - src.ReservedQuantity))
                .ForMember(dest => dest.IsInStock,
                    opt => opt.MapFrom(src => (src.StockQuantity - src.ReservedQuantity) > 0));

            CreateMap<CreateMerchandiseDto, Merchandise>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => SerializeImages(src.Images)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsOfficial, opt => opt.Ignore())
                .ForMember(dest => dest.SellerId, opt => opt.Ignore())
                .ForMember(dest => dest.Seller, opt => opt.Ignore())
                .ForMember(dest => dest.Variants, opt => opt.Ignore());

            CreateMap<UpdateMerchandiseDto, Merchandise>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => SerializeImages(src.Images)))
                .ForMember(dest => dest.SKU, opt => opt.Ignore())
                .ForMember(dest => dest.IsOfficial, opt => opt.Ignore())
                .ForMember(dest => dest.SellerId, opt => opt.Ignore())
                .ForMember(dest => dest.Seller, opt => opt.Ignore())
                .ForMember(dest => dest.Variants, opt => opt.Ignore())
                .ForMember(dest => dest.ReservedQuantity, opt => opt.Ignore());

            // MerchandiseVariant mappings
            CreateMap<MerchandiseVariant, MerchandiseVariantDto>()
                .ForMember(dest => dest.FinalPrice,
                    opt => opt.MapFrom((src, dest, destMember, context) =>
                    {
                        var basePrice = context.Items.ContainsKey("BasePrice")
                            ? (decimal)context.Items["BasePrice"]
                            : (src.Merchandise?.BasePrice ?? 0);
                        return basePrice + src.PriceModifier;
                    }))
                .ForMember(dest => dest.IsInStock,
                    opt => opt.MapFrom(src => src.StockQuantity > 0));

            CreateMap<CreateVariantDto, MerchandiseVariant>()
                .ForMember(dest => dest.MerchandiseId, opt => opt.Ignore())
                .ForMember(dest => dest.Merchandise, opt => opt.Ignore());

            CreateMap<UpdateVariantDto, MerchandiseVariant>()
                .ForMember(dest => dest.MerchandiseId, opt => opt.Ignore())
                .ForMember(dest => dest.Merchandise, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.Ignore());
        }

        private List<string> DeserializeImages(string imagesJson)
        {
            if (string.IsNullOrEmpty(imagesJson))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(imagesJson) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private string SerializeImages(List<string> images)
        {
            if (images == null || images.Count == 0)
                return "[]";

            return JsonSerializer.Serialize(images);
        }
    }
}