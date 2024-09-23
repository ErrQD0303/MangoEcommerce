using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.Repositories
{
    public interface IRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity?> CreateAsync(TEntity entity);
        Task<TEntity?> UpdateAsync(int id, TEntity entity);
        Task<TEntity?> DeleteAsync(int id);
    }
}