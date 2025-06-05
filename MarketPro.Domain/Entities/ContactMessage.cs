using System;
using System.ComponentModel.DataAnnotations;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class ContactMessage : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
} 