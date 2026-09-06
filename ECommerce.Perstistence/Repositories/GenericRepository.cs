using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain;
using ECommerce.Domain.Contracts;
using ECommerce.Perstistence.Data.DbContexts;
using ECommerce.Services.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Perstistence.Repositories
{
    internal class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>, new()
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity) => await _dbContext.AddAsync(entity);

        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            ISpecifications<TEntity, TKey> specifications
        )
        {
            var query = SpecificationEvaluator.CreateQuery(
                _dbContext.Set<TEntity>(),
                specifications
            );

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id) =>
            await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var query = SpecificationEvaluator.CreateQuery(
                _dbContext.Set<TEntity>(),
                specifications
            );
            return await query.FirstOrDefaultAsync();
        }

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);
    }
}
