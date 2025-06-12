using MarketPro.Application.DTOs;
using MarketPro.Application.DTOs.Product;
using System.Collections.Generic;

namespace MarketPro.Models
{
    public class ProductListViewModel
    {
        public IList<ProductDto> Products { get; set; }
        public IList<ProductTypeDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
} 