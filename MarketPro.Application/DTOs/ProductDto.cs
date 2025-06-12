using System.Collections.Generic;

namespace MarketPro.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string ShortDescription { get; set; }
        public decimal Rating { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
        public int OrderCount { get; set; }
    }
} 