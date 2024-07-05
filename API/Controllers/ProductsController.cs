
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _ProductBrandRepos;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, 
        IGenericRepository<ProductType> productTypeRepo, 
        IGenericRepository<ProductBrand> productBrandRepos,
        IMapper mapper)
        {
            _ProductBrandRepos = productBrandRepos;
            _mapper = mapper;
            _productRepo = productRepo;
            _productTypeRepo = productTypeRepo;
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>>  GetProducts()
        {
            var spec = new ProductsWithBrandAndTypeSpecification();
            var products = await _productRepo.ListAsync(spec);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>>  GetProduct(int id)
        {
             var spec = new ProductsWithBrandAndTypeSpecification(id);
             var product = await _productRepo.GetEntityWithSpec(spec);
             if(product == null) return NotFound(new ApiResponse(404));

             return  _mapper.Map<Product, ProductDto>(product);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>>  GetProductBrands()
        {
            return  Ok(await _ProductBrandRepos.ListAllAsync());
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>>  GetProductTypes()
        {
            return  Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}