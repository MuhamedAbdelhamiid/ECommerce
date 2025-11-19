using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;

namespace ECommerce.Services.Specifications.Product_Specifications
{
    internal class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithBrandAndTypeSpecifications(ProductQueryParams productQuery)
            : base(x =>
                (!productQuery.typeId.HasValue || x.ProductTypeId == productQuery.typeId)
                && (!productQuery.brandId.HasValue || x.ProductBrandId == productQuery.brandId)
                && (
                    string.IsNullOrEmpty(productQuery.search)
                    || x.Name.ToLower().Contains(productQuery.search)
                )
            )
        {
            AddInclude(X => X.ProductBrand);
            AddInclude(X => X.ProductType);

            switch (productQuery.sort)
            {
                case ProductOrderOption.NameAsc:
                    ApplyOrderBy(X => X.Name);
                    break;
                case ProductOrderOption.NameDesc:
                    ApplyOrderByDescending(X => X.Name);
                    break;
                case ProductOrderOption.PriceAsc:
                    ApplyOrderBy(X => X.Price);
                    break;
                case ProductOrderOption.PriceDesc:
                    ApplyOrderByDescending(X => X.Price);
                    break;
                default:
                    ApplyOrderBy(X => X.Id);
                    break;
            }

            ApplyPagination(productQuery.pageIndex, productQuery.pageSize);
        }

        public ProductWithBrandAndTypeSpecifications(int id)
            : base(x => x.Id == id)
        {
            AddInclude(X => X.ProductBrand);
            AddInclude(X => X.ProductType);
        }
    }
}
