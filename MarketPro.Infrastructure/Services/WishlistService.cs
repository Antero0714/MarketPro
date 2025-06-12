using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using MarketPro.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketPro.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;

        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<WishlistItem> GetWishlistItems(string userId)
        {
            return _context.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .Where(w => w.UserId == userId)
                .ToList();
        }

        public bool AddToWishlist(string userId, int productId)
        {
            var existingItem = _context.WishlistItems
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem != null)
            {
                return false; // Item already exists in wishlist
            }

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                AddedDate = DateTime.UtcNow
            };

            _context.WishlistItems.Add(wishlistItem);
            _context.SaveChanges();
            return true;
        }

        public void RemoveFromWishlist(string userId, int productId)
        {
            var wishlistItem = _context.WishlistItems
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                _context.SaveChanges();
            }
        }
    }
} 