using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;

namespace ECommerce.Services.Specifications
{
    public class CountOfProductWithSpecifications : BaseSpecifications<Product, int>
    {
        public CountOfProductWithSpecifications(ProductQueryParams queryParams)
            : base(p =>
                ((!queryParams.brandId.HasValue) || (p.ProductBrandId == queryParams.brandId))
                && ((!queryParams.typeId.HasValue) || (p.ProductTypeId == queryParams.typeId))
                && (
                    string.IsNullOrEmpty(queryParams.search)
                    || p.Name.ToLower().Contains(queryParams.search.ToLower())
                )
            ) { }
    }
}
