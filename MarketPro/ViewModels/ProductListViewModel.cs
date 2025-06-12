using MarketPro.Domain.Entities;
using System.Collections.Generic;

namespace MarketPro.ViewModels
{
    public class ProductListViewModel
    {
        public IList<Product> Products { get; set; }
        public IList<ProductType> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
} 