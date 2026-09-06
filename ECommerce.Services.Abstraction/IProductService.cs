using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.ProductModuleDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IProductService
    {
        Task<Result<PaginatedResult<ProductDTO>>> GetAllProductsAsync(
            ProductQueryParams productQueryParams
        );
        Task<Result<ProductDTO?>> GetProductByIdAsync(int id);
        Task<Result<IEnumerable<TypeDTO>>> GetAllTypesAsync();
        Task<Result<IEnumerable<BrandDTO>>> GetAllBrandsAsync();
    }
}
