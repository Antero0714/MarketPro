using MarketPro.Application.DTOs.Product;
using System.Collections.Generic;

namespace MarketPro.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderItem> SliderItems { get; set; } = new List<SliderItem>();
        public List<ProductDto> FeaturedProducts { get; set; } = new List<ProductDto>();
        public IEnumerable<ProductDto> DealProducts { get; set; }
        public IEnumerable<ProductDto> RecommendedProducts { get; set; }
        public IEnumerable<ProductTypeDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public List<int> CartItemIds { get; set; } = new List<int>();
        public List<int> WishlistItemIds { get; set; } = new List<int>();
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