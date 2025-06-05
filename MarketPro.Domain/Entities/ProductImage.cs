using System.ComponentModel.DataAnnotations;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public bool IsPrimary { get; set; }
    }
} 