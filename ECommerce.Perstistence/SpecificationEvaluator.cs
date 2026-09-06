using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain;
using ECommerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ECommerce.Perstistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(
            IQueryable<TEntity> entryPoint,
            ISpecifications<TEntity, TKey> specifications
        )
            where TEntity : BaseEntity<TKey>, new()
        {
            var query = entryPoint;

            if (specifications is not null)
            {
                if (specifications.Criteria is not null)
                    query = query.Where(specifications.Criteria);
                if (
                    specifications.IncludesExpressions is not null
                    && specifications.IncludesExpressions.Any()
                )
                {
                    foreach (var exp in specifications.IncludesExpressions)
                    {
                        query = query.Include(exp);
                    }
                }
                if (specifications.OrderBy is not null)
                    query = query.OrderBy(specifications.OrderBy);
                if (specifications.OrderByDescending is not null)
                    query = query.OrderByDescending(specifications.OrderByDescending);

                if (specifications.IsPaginated)
                    query = query.Skip(specifications.Skip).Take(specifications.Take);
            }

            return query;
        }
    }
}
