using API.Helpers.QueryObjects.Products;
using API.Interfaces.Exists;
using API.Models.Accounts;
using API.Models.Products;

namespace API.Interfaces.Repositories.Accounts
{
    public interface IAccountRepository : IRepository<AppUser>
    {
        Task<IEnumerable<Product>> GetAllWithQueryAsync(ProductQueryObject queryObject);
    }
}