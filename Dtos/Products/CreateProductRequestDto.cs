using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.DataAnnotations;

namespace API.Dtos.Products
{
    public class CreateProductRequestDto
    {
        [MaxLengthWithErrorMessage(255)]
        public string Name { get; set; } = string.Empty;
        [MaxLengthWithErrorMessage(1000)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [MaxLengthWithErrorMessage(255)]
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        [MaxLengthWithErrorMessage(255)]
        public string Origin { get; set; } = string.Empty;
    }
}