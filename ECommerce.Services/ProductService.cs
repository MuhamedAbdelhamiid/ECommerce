using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications;
using ECommerce.Shared;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.ProductModuleDTOs;

namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<BrandDTO>>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();

            if (brands is null || !brands.Any())
                return Result<IEnumerable<BrandDTO>>.Fail(Error.NotFound("Brands Not Found"));

            var brandsToReturn = _mapper.Map<IEnumerable<BrandDTO>>(brands);
            return Result<IEnumerable<BrandDTO>>.Ok(brandsToReturn);
        }

        public async Task<Result<PaginatedResult<ProductDTO>>> GetAllProductsAsync(
            ProductQueryParams productQueryParams
        )
        {
            var spec = new ProductWithSpecifications(productQueryParams);
            var products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);

            var countSpec = new CountOfProductWithSpecifications(productQueryParams);
            var rowProductsWithoutAnyPagination = await _unitOfWork
                .GetRepository<Product, int>()
                .GetAllAsync(countSpec);

            if (products is null || !products.Any())
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    Error.NotFound(description: "Products Not Found")
                );
            ;
            var productToReturn = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                new PaginatedResult<ProductDTO>(
                    productQueryParams.PageIndex,
                    productToReturn.Count(),
                    rowProductsWithoutAnyPagination.Count(),
                    productToReturn
                )
            );
        }

        public async Task<Result<IEnumerable<TypeDTO>>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();

            if (types is null || !types.Any())
                return Result<IEnumerable<TypeDTO>>.Fail(
                    Error.NotFound(description: "Types not found")
                );

            var typesToReturn = _mapper.Map<IEnumerable<TypeDTO>>(types);
            return Result<IEnumerable<TypeDTO>>.Ok(typesToReturn);
        }

        public async Task<Result<ProductDTO?>> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithSpecifications(id);
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);

            return product is not null
                ? Result<ProductDTO?>.Ok(_mapper.Map<ProductDTO>(product))
                : Result<ProductDTO?>.Fail(Error.NotFound(description: "Product Not Found"));
        }
    }
}
