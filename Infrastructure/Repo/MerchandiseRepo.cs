using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class MerchandiseRepo: Repo<Merchandise>, IMerchandiseRepo
    {
        public MerchandiseRepo(AppDbContext context) : base(context)
        {
        }

       public async Task<IEnumerable<Merchandise>> GetActiveProductsAsync(int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Merchandises
                .Where(m => !m.IsDeleted && m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Merchandise>> GetBySellerAsync(int sellerId)
        {
            return await _context.Merchandises
                .Where(m => !m.IsDeleted && m.SellerId == sellerId)
                .Include(m => m.Variants)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Merchandise>> GetOfficialProductsAsync()
        {
            return await _context.Merchandises
                .Where(m => !m.IsDeleted && m.IsActive && m.IsOfficial)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Merchandise?> GetBySkuAsync(string sku)
        {
            return await _context.Merchandises
                .FirstOrDefaultAsync(m => m.SKU == sku && !m.IsDeleted);
        }

        public async Task<Merchandise?> GetWithDetailsAsync(int id)
        {
            return await _context.Merchandises
                .Include(m => m.Seller)
                .Include(m => m.Variants.Where(v => !v.IsDeleted))
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<IEnumerable<Merchandise>> SearchAsync(string searchTerm)
        {
            return await _context.Merchandises
                .Where(m => !m.IsDeleted && m.IsActive &&
                    (m.Name.Contains(searchTerm) ||
                     m.Description.Contains(searchTerm) ||
                     m.SKU.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Merchandise>> GetByCategoryAsync(string category)
        {
            return await _context.Merchandises
                .Where(m => !m.IsDeleted && m.IsActive && m.Category == category)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}