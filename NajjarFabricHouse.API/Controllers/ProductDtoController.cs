using Microsoft.AspNetCore.Mvc;
using NajjarFabricHouse.Data.Models;

using NajjarFabricHouse.Service.Services;



namespace NajjarFabricHouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDtoController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductDtoController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string filterBy, string sortBy, bool isAscending = true)
        {
            var response = await _productRepository.GetAllProductsAsync(filterBy, sortBy, isAscending);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response.Message);

            return Ok(response.Response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productRepository.GetProductByIdAsync(id);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response.Message);

            return Ok(response.Response);
        }


        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product productDto)
        {
            var response = await _productRepository.CreateProductAsync(productDto);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response.Message);

            return Ok(response.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product productDto)
        {
            var response = await _productRepository.UpdateProductAsync(id, productDto);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response.Message);

            return Ok(response.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productRepository.DeleteProductAsync(id);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response.Message);

            return Ok(response.Message);
        }
    }
}
