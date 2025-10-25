using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repo
{
    public class MerchandiseVariantRepo : Repo<MerchandiseVariant>, IMerchandiseVariantRepo
    {
        public MerchandiseVariantRepo(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<MerchandiseVariant>> GetByMerchandiseIdAsync(int merchandiseId)
        {
            return await _context.MerchandiseVariants
                .Where(v => v.MerchandiseId == merchandiseId && !v.IsDeleted)
                .ToListAsync();
        }

        public async Task<MerchandiseVariant?> GetWithMerchandiseAsync(int variantId)
        {
            return await _context.MerchandiseVariants
                .Include(v => v.Merchandise)
                .FirstOrDefaultAsync(v => v.Id == variantId && !v.IsDeleted);
        }
    }
}
