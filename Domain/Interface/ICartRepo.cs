using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ICartRepo :IRepo<CartModel>
    {
        Task<CartModel?> GetActiveCartByUserIdAsync(int userId);
        Task<CartModel?> GetCartWithItemsAsync(int cartId);
        Task CleanupExpiredCartsAsync();
    }
}
