using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IShippingConfigRepo: IRepo<ShippingConfig>
    {
        Task<ShippingConfig?> GetConfigByCityAsync(string city);
        Task<ShippingConfig?> GetDefaultConfigAsync();
        Task<IEnumerable<ShippingConfig>> GetActiveConfigsAsync();
        Task<bool> IsCityConfigExistsAsync(string city);
    }
}
