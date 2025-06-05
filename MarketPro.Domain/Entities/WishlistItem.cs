using System;
using System.ComponentModel.DataAnnotations;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class WishlistItem : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
} 