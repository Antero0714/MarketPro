using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string ShortDescription { get; set; }

        public string DetailedDescription { get; set; }

        public string Specifications { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; }

        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        // Navigation properties
        public ICollection<ProductImage> Images { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
} 