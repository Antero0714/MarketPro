using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MarketPro.Infrastructure.Hubs
{
    public class OrderHub : Hub
    {
        private readonly ILogger<OrderHub> _logger;

        public OrderHub(ILogger<OrderHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to OrderHub: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected from OrderHub with error: {ErrorMessage}", exception.Message);
            }
            else
            {
                _logger.LogInformation("Client disconnected from OrderHub: {ConnectionId}", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendOrderNotification(string customerName, int orderId, string status)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", customerName, orderId, status);
        }
    }
} 