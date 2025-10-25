using Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs.ShippingConfig;
using Domain.Entities.MerchandiseEntity;
namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureOrderMappings()
        {
            CreateMap<OrderFilterDto, OrderFilter>();
            CreateMap<UserOrderStatistics, UserOrderStatDto>();


            CreateMap<OrderModel, OrderDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplay,
                    opt => opt.MapFrom(src => GetStatusDisplay(src.Status)))
                .ForMember(dest => dest.ShippingAddress,
                    opt => opt.MapFrom(src => DeserializeShippingAddress(src.ShippingAddress)))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderModel, OrderListDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplay,
                    opt => opt.MapFrom(src => GetStatusDisplay(src.Status)))
                .ForMember(dest => dest.ItemCount,
                    opt => opt.MapFrom(src => src.OrderItems.Count));

            CreateMap<OrderItemModel, OrderItemDto>()
                .ForMember(dest => dest.MerchandiseName,
                    opt => opt.MapFrom(src => src.Merchandise.Name))
                .ForMember(dest => dest.MerchandiseSKU,
                    opt => opt.MapFrom(src => src.Merchandise.SKU))
                .ForMember(dest => dest.VariantName,
                    opt => opt.MapFrom(src => src.Variant != null
                        ? $"{src.Variant.Name}: {src.Variant.Value}"
                        : null))
                .ForMember(dest => dest.ThumbnailImage,
                    opt => opt.MapFrom(src => DeserializeImages(src.Merchandise.Images).FirstOrDefault()));
        }

        //deserialize ShippingAddress
        private ShippingAddressDto? DeserializeShippingAddress(string shippingAddressJson)
        {
            if (string.IsNullOrEmpty(shippingAddressJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<ShippingAddressDto>(shippingAddressJson);
            }
            catch
            {
                return null;
            }
        }

        private string GetStatusDisplay(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Đang chờ xử lý",
                OrderStatus.Processing => "Đang xử lý",
                OrderStatus.Shipped => "Đang giao hàng",
                OrderStatus.Delivered => "Đã giao hàng",
                OrderStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

    }
}