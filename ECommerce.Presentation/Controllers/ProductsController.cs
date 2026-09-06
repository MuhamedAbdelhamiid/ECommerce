using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Presentation.Attributes;
using ECommerce.Services.Abstraction;
using ECommerce.Shared;
using ECommerce.Shared.DTOs.ProductModuleDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class ProductsController : ApiBaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet()]
        //[Authorize(Roles = "SuperAdmin")]
        [RedisCache(5)]
        public async Task<ActionResult<PaginatedResult<ProductDTO>>> GetAllProducts(
            [FromQuery] ProductQueryParams productQueryParams
        )
        {
            var result = await _productService.GetAllProductsAsync(productQueryParams);
            return HandleResult(result);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
        {
            var result = await _productService.GetAllTypesAsync();
            return HandleResult(result);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAllBrands()
        {
            var result = await _productService.GetAllBrandsAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return HandleResult(result);
        }
    }
}
