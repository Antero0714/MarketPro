using System.Collections.Generic;
using MarketPro.Application.DTOs;
using MarketPro.Application.DTOs.Product;

namespace MarketPro.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<SliderItem> SliderItems { get; set; }
        public IEnumerable<ProductDto> DealProducts { get; set; }
        public IEnumerable<ProductDto> TopSellingProducts { get; set; }
        public IEnumerable<ProductDto> TrendingProducts { get; set; }
        public IEnumerable<ProductDto> FeaturedProducts { get; set; }
        public IEnumerable<ProductDto> RecommendedProducts { get; set; }
        public IEnumerable<ProductTypeDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public IEnumerable<int> CartItemIds { get; set; }
        public IEnumerable<int> WishlistItemIds { get; set; }
        //public BannerViewModel Banner { get; set; }
    }

    public class SliderItem
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string ButtonText { get; set; } = "Shop Now";
        public string ButtonUrl { get; set; }
    }
} 