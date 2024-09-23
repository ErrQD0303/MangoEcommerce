using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces.Exists;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IExistenceCheck
    where TEntity : class
    {
        internal readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity?> CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity?> DeleteAsync(int id)
        {
            var model = await GetByIdAsync(id);

            if (model == null)
            {
                return null;
            }

            _context.Set<TEntity>().Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<TEntity>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().Skip(0).Take(20).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity?> UpdateAsync(int id, TEntity entity)
        {
            var existingEntity = await GetByIdAsync(id);

            if (existingEntity == null) return null;

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existingEntity;
        }
    }
}