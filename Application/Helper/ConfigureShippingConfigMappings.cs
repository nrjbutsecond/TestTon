using Application.DTOs.ShippingConfig;
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
        private void ConfigureShippingConfigMappings()
        {
            CreateMap<ShippingConfig, ShippingConfigDto>();
            
            CreateMap<CreateShippingConfigDto, ShippingConfig>()
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateShippingConfigDto, ShippingConfig>()
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
