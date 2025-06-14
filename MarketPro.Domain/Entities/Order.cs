using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string BusinessName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(200)]
        public string HouseNumberAndStreet { get; set; }

        [MaxLength(100)]
        public string Apartment { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string AdditionalInformation { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; }
    }
} 