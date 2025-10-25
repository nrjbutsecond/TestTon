using Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveFromCartAsync(int userId, int itemId);
        Task<CartDto> ClearCartAsync(int userId);
    }
}
