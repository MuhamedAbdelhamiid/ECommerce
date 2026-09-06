using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain;
using ECommerce.Domain.Contracts;
using ECommerce.Perstistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace ECommerce.Perstistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext;
        private Dictionary<Type, object> repositories = [];

        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>, new()
        {
            if (
                repositories.TryGetValue(
                    typeof(GenericRepository<TEntity, TKey>),
                    out var repository
                )
            )
                return (IGenericRepository<TEntity, TKey>)repository;

            var newRepo = new GenericRepository<TEntity, TKey>(_dbContext);
            repositories.Add(typeof(GenericRepository<TEntity, TKey>), newRepo);
            return newRepo;
        }

        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
