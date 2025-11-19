using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Specifications.Product_Specifications
{
    internal class ProductWithCountSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithCountSpecifications(ProductQueryParams productQuery) :
            base(x =>
                (!productQuery.typeId.HasValue || x.ProductTypeId == productQuery.typeId)
                && (!productQuery.brandId.HasValue || x.ProductBrandId == productQuery.brandId)
                && (
                    string.IsNullOrEmpty(productQuery.search)
                    || x.Name.ToLower().Contains(productQuery.search)
                )
            )
        {
            
        }
    }
}
