using System;
using System.Collections.Generic;

namespace MarketPro.WebAPI.Areas.AdminPanel.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalWishlistItems { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCartItems { get; set; }
        public List<OrderDetailsViewModel> RecentOrders { get; set; }
    }
} 