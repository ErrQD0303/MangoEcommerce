using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums.Products;

namespace API.Helpers.QueryObjects.Products
{
    public class ProductQueryObject : QueryObject
    {
        public string Name { get; set; } = string.Empty;
        public decimal? Weight { get; set; } = null;
        public string Categories { get; set; } = string.Empty; // Seperated by semicolon ";"
        public ProductSortBy SortBy { get; set; } = ProductSortBy.Name;
    }
}