using API.Helpers.QueryObjects.Products;
using API.Interfaces.Exists;
using API.Models.Products;

namespace API.Interfaces.Repositories.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithQueryAsync(ProductQueryObject queryObject);
    }
}