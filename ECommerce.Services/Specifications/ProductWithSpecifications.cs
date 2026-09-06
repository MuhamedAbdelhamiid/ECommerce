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
    public class ProductWithSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithSpecifications(ProductQueryParams queryParams)
            : base(p =>
                ((!queryParams.brandId.HasValue) || (p.ProductBrandId == queryParams.brandId))
                && ((!queryParams.typeId.HasValue) || (p.ProductTypeId == queryParams.typeId))
                && (
                    string.IsNullOrEmpty(queryParams.search)
                    || p.Name.ToLower().Contains(queryParams.search.ToLower())
                )
            )
        {
            AddInclude(x => x.ProductBrand);
            AddInclude(x => x.ProductType);

            switch (queryParams.sort)
            {
                case SortOptions.NameAsc:
                    AddOrderBy(X => X.Name);
                    break;
                case SortOptions.NameDesc:
                    AddOrderByDescending(X => X.Name);
                    break;
                case SortOptions.PriceAsc:
                    AddOrderBy(X => X.Price);
                    break;
                case SortOptions.PriceDesc:
                    AddOrderByDescending(X => X.Price);
                    break;
                default:
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }

        public ProductWithSpecifications(int id)
            : base(X => X.Id == id) { }

        public ProductWithSpecifications(List<int> selectedProductsId)
            : base(x => selectedProductsId.Contains(x.Id)) { }
    }
}
