using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class ProductType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
} 