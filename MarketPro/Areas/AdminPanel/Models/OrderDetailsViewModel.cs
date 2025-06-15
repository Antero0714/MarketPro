using System;
using System.Collections.Generic;

namespace MarketPro.WebAPI.Areas.AdminPanel.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
} 