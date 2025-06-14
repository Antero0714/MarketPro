using MarketPro.Application.Interfaces.Services;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MarketPro.Infrastructure.Services
{
    public class OrderService 
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Получаем все товары из корзины пользователя
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                // Создаем заказ
                order.UserId = userId;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";
                order.Notes = order.Notes ?? string.Empty; // Set default value if null

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Создаем элементы заказа из корзины
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}