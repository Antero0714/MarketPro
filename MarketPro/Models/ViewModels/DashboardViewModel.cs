using System;

namespace MarketPro.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalWishlistItems { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCartItems { get; set; }
    }
} 