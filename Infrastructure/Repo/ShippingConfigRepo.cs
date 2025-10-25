using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.MerchandiseEntity;

namespace Infrastructure.Repo
{
    public class ShippingConfigRepo: Repo<ShippingConfig>, IShippingConfigRepo
    {
        public ShippingConfigRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<ShippingConfig?> GetConfigByCityAsync(string city)
        {
            return await _context.shippingConfigs
                .FirstOrDefaultAsync(sc =>
                    sc.City.ToLower() == city.ToLower() &&
                    sc.IsActive &&
                    !sc.IsDefault);
        }

        public async Task<ShippingConfig?> GetDefaultConfigAsync()
        {
            return await _context.shippingConfigs
                .FirstOrDefaultAsync(sc => sc.IsDefault && sc.IsActive);
        }

        public async Task<IEnumerable<ShippingConfig>> GetActiveConfigsAsync()
        {
            return await _context.shippingConfigs
                .Where(sc => sc.IsActive)
                .OrderBy(sc => sc.IsDefault)
                .ThenBy(sc => sc.City)
                .ToListAsync();
        }

        public async Task<bool> IsCityConfigExistsAsync(string city)
        {
            return await _context.shippingConfigs
                .AnyAsync(sc =>
                    sc.City.ToLower() == city.ToLower() &&
                    sc.IsActive &&
                    !sc.IsDefault);
        }
    }
}
