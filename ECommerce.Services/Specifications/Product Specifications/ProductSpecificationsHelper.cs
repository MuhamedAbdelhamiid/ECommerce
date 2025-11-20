using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;

namespace ECommerce.Services.Specifications.Product_Specifications
{
    internal static class ProductSpecificationsHelper
    {
        public static Expression<Func<Product, bool>> GetCriteria(ProductQueryParams productQuery)
        {
            return x =>
                (!productQuery.typeId.HasValue || x.ProductTypeId == productQuery.typeId)
                && (!productQuery.brandId.HasValue || x.ProductBrandId == productQuery.brandId)
                && (
                    string.IsNullOrEmpty(productQuery.search)
                    || x.Name.ToLower().Contains(productQuery.search)
                );
        }
    }
}
