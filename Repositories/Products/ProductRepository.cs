using API.Data;
using API.Extensions.Query;
using API.Helpers.QueryObjects.Products;
using API.Interfaces.Repositories.Products;
using API.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Products
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithQueryAsync(ProductQueryObject queryObject)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Name))
            {
                products = products.Where(p => p.Name == queryObject.Name);
            }

            if (queryObject.Weight != null)
            {
                products = products.Where(p => p.Weight == queryObject.Weight);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Categories))
            {
            }

            return await products
                .ApplySorting(queryObject.SortBy.ToString(), queryObject.IsDescending)
                .ApplyPagination(queryObject.PageNumber, queryObject.PageSize)
                .ToListAsync();
        }
    }
}