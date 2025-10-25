using Application.DTOs.Cart;
using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureCartMappings()
        {
            CreateMap<CartModel, CartDto>()
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
        .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src =>
            src.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity)))
        .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src =>
            src.CartItems.Sum(ci => ci.Quantity)));

            CreateMap<CartItemModel, CartItemDto>()
                .ForMember(dest => dest.MerchandiseName, opt => opt.MapFrom(src => src.Merchandise.Name))
                .ForMember(dest => dest.MerchandiseSKU, opt => opt.MapFrom(src => src.Merchandise.SKU))
                .ForMember(dest => dest.ThumbnailImage, opt => opt.MapFrom(src =>
                    DeserializeImages(src.Merchandise.Images).FirstOrDefault()))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src =>
                    src.Variant != null ? $"{src.Variant.Name}: {src.Variant.Value}" : null))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity))
                .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src =>
                    src.VariantId.HasValue
                        ? src.Variant!.StockQuantity
                        : src.Merchandise.StockQuantity - src.Merchandise.ReservedQuantity));
        }
    }
}
