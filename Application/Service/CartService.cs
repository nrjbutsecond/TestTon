using Application.DTOs.Cart;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int CartExpirationHours = 24;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            // Get merchandise with stock info
            var merchandise = await _unitOfWork.Merchandises.GetWithDetailsAsync(dto.MerchandiseId);
            if (merchandise == null || !merchandise.IsActive)
                throw new InvalidOperationException("Product not available");

            // Calculate price and check stock
            decimal unitPrice = merchandise.BasePrice;
            int availableStock = merchandise.StockQuantity - merchandise.ReservedQuantity;

            if (dto.VariantId.HasValue)
            {
                var variant = merchandise.Variants.FirstOrDefault(v => v.Id == dto.VariantId);
                if (variant == null)
                    throw new InvalidOperationException("Variant not found");

                unitPrice += variant.PriceModifier;
                availableStock = variant.StockQuantity;
            }

            // Check existing item
            var existingItem = cart.CartItems.FirstOrDefault(ci =>
                ci.MerchandiseId == dto.MerchandiseId &&
                ci.VariantId == dto.VariantId);

            if (existingItem != null)
            {
                // Update quantity
                var newQuantity = existingItem.Quantity + dto.Quantity;
                if (newQuantity > availableStock)
                    throw new InvalidOperationException("Insufficient stock");

                existingItem.Quantity = newQuantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new item
                if (dto.Quantity > availableStock)
                    throw new InvalidOperationException("Insufficient stock");

                var cartItem = new CartItemModel
                {
                    CartId = cart.Id,
                    MerchandiseId = dto.MerchandiseId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity,
                    UnitPrice = unitPrice,
                    CreatedAt = DateTime.UtcNow
                };

                cart.CartItems.Add(cartItem);
            }

            // Update merchandise reserved quantity
            merchandise.ReservedQuantity += dto.Quantity;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto dto)
        {
            var cart = await _unitOfWork.Carts.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart");

            // Check stock
            var merchandise = item.Merchandise;
            int availableStock = item.VariantId.HasValue
                ? item.Variant!.StockQuantity
                : merchandise.StockQuantity - merchandise.ReservedQuantity + item.Quantity;

            if (dto.Quantity > availableStock)
                throw new InvalidOperationException("Insufficient stock");

            // Update reserved quantity
            var quantityDiff = dto.Quantity - item.Quantity;
            merchandise.ReservedQuantity += quantityDiff;

            item.Quantity = dto.Quantity;
            item.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RemoveFromCartAsync(int userId, int itemId)
        {
            var cart = await _unitOfWork.Carts.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart");

            // Release reserved quantity
            item.Merchandise.ReservedQuantity -= item.Quantity;

            cart.CartItems.Remove(item);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return new CartDto();

            // Release all reserved quantities
            foreach (var item in cart.CartItems)
            {
                item.Merchandise.ReservedQuantity -= item.Quantity;
            }

            cart.CartItems.Clear();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CartDto>(cart);
        }

        private async Task<CartModel> GetOrCreateCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetActiveCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new CartModel
                {
                    UserId = userId,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // ✅ luôn dùng UTC
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            return cart;
        }
    }
}