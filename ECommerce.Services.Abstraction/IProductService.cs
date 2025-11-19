using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared;
using ECommerce.Shared.DTOs.ProductDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IProductService
    {
        Task<PagintedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams productQuery);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
    }
}
