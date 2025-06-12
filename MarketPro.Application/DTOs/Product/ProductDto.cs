using System.Collections.Generic;

namespace MarketPro.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string DetailedDescription { get; set; }
        public decimal Rating { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string StoreName { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }
        public string Specifications { get; set; }
        public int OrderItemsCount { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
    }
} 