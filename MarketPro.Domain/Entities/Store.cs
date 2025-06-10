using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MarketPro.Domain.Common;

namespace MarketPro.Domain.Entities
{
    public class Store : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
} 