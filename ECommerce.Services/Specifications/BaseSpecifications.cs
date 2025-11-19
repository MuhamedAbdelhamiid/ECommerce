using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;

namespace ECommerce.Services.Specifications
{
    internal abstract class BaseSpecifications<TEntity, Tkey> : ISpecifications<TEntity, Tkey>
        where TEntity : BaseEntity<Tkey>, new()
    {
        protected BaseSpecifications(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }


        #region Filteration
        public Expression<Func<TEntity, bool>> Criteria { get; }

        #endregion

        #region Ordering
        public Expression<Func<TEntity, object>> OrderBy { private set; get; }

        public Expression<Func<TEntity, object>> OrderByDescending { private set; get; }

        public void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExp)
        {
            OrderBy = orderByExp;
        }
        public void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescExp)
        {
            OrderByDescending = orderByDescExp;
        }

        #endregion

        #region Includes
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        } 
        #endregion
    }
}
