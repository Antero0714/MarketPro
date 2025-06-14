using MarketPro.Application.DTOs;
using MarketPro.Application.DTOs.Product;
using System.Collections.Generic;

namespace MarketPro.Models
{
    public class CatalogViewModel
    {
        public IList<ProductDto> Products { get; set; }
        public IList<ProductTypeDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public IEnumerable<int> CartItemIds { get; set; }
        public IEnumerable<int> WishlistItemIds { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Rating { get; set; }
        public string Brand { get; set; }
    }
} 