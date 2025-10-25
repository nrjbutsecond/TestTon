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
    public class CartRepo : Repo<CartModel>, ICartRepo

    {
        public CartRepo(AppDbContext context) : base(context)
        {

        }
        public async Task<CartModel?> GetActiveCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Merchandise)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Variant)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<CartModel?> GetCartWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Merchandise)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Variant)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task CleanupExpiredCartsAsync()
        {
            var expiredCarts = await _context.Carts
                .Where(c => c.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.Carts.RemoveRange(expiredCarts);
        }
    }
}

