namespace MarketPro.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public double Rating { get; set; }
        public string ProductTypeName { get; set; }
        public string StoreName { get; set; }
        public string MainImageUrl { get; set; }
    }
} 