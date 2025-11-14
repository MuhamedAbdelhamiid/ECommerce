using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerce.Persistence
{
    internal static class SpecificationsEvaluator
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
                {
                    query = query.Where(specifications.Criteria);
                }
                if (
                    specifications.IncludeExpressions is not null
                    && specifications.IncludeExpressions.Any()
                )
                {
                    query = specifications.IncludeExpressions.Aggregate(
                        query,
                        (currentQuery, includeExp) => currentQuery.Include(includeExp)
                    );
                }
            }

            return query;
        }
    }
}
