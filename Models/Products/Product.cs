using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Products
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Weight { get; set; }
        [MaxLength(255)]
        public string Origin { get; set; } = string.Empty;
    }
}