using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(TKey id);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}
