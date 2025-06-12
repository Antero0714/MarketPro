using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using MarketPro.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPro.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CartItem> GetCartItems(string userId)
        {
            return _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Store)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public bool AddToCart(string userId, int productId)
        {
            var existingItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return true;
        }

        public void RemoveFromCart(string userId, int productId)
        {
            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public void UpdateQuantity(string userId, int productId, int quantity)
        {
            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }
        }

        public decimal GetCartTotal(string userId)
        {
            return _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Sum(c => c.Product.Price * c.Quantity);
        }
    }
} 