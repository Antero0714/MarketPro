using MarketPro.Application.Interfaces.Services;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MarketPro.Infrastructure.Hubs;
using Microsoft.Extensions.Logging;

namespace MarketPro.Infrastructure.Services
{
    public class OrderService 
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext context, 
            IHubContext<OrderHub> hubContext,
            ILogger<OrderService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
            _logger.LogInformation("OrderService initialized with SignalR hub");
        }

        public async Task<Order> CreateOrderAsync(Order order, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting to create order for user {UserId}", userId);

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
                order.Notes = order.Notes ?? string.Empty;

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

                // Send SignalR notification
                try
                {
                    _logger.LogInformation("Attempting to send SignalR notification for Order #{OrderId} from {CustomerName}", 
                        order.Id, $"{order.FirstName} {order.LastName}");

                    if (_hubContext == null)
                    {
                        _logger.LogError("HubContext is null - SignalR notification cannot be sent");
                    }
                    else
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification",
                            $"{order.FirstName} {order.LastName}",
                            order.Id,
                            order.Status);

                        _logger.LogInformation("Successfully sent SignalR notification");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SignalR notification. Error: {ErrorMessage}", ex.Message);
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
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