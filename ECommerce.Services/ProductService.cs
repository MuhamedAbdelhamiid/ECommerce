using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications.Product_Specifications;
using ECommerce.Shared;
using ECommerce.Shared.DTOs.ProductDTOs;

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

        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();

            return _mapper.Map<IEnumerable<BrandDTO>>(brands);
        }

        public async Task<PagintedResult<ProductDTO>> GetAllProductsAsync(
            ProductQueryParams productQuery
        )
        {
            var productRepo = _unitOfWork.GetRepository<Product, int>();

            var spec = new ProductWithBrandAndTypeSpecifications(productQuery);
            var products = await productRepo.GetAllAsync(spec);
            

            var countSpec = new ProductWithCountSpecifications(productQuery);
            var countOfFilteredData = await productRepo.CountAsync(countSpec);


            var dataToReturn = _mapper.Map<IEnumerable<ProductDTO>>(products);
            var countOfDataToReturn = dataToReturn.Count();


            return new PagintedResult<ProductDTO>(productQuery.pageIndex, countOfDataToReturn, countOfFilteredData, dataToReturn);
        }

        public async Task<IEnumerable<TypeDTO>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<TypeDTO>>(types);
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);

            return _mapper.Map<ProductDTO>(product);
        }
    }
}
