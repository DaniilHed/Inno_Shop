using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Aplication.Interafaces;
using ProductManagmentMicroservice.Aplication.Services;

namespace ProductManagmentMicroservice.API
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll() 
		{
			var products=await _productService.GetAllProductsAsync();
			return Ok(products);
		}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var product = await _productService.GetProductById(id);
			return Ok(product);
		}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetProductsByUserId(Guid userId)
		{
			var products = await _productService.GetProductsByUserIdAsync(userId);

			if (!products.Any())
			{
				return NotFound("No products found for this user.");
			}

			return Ok(products);
		}
		[HttpGet("filter")]
		public async Task<IActionResult> GetFilteredProducts([FromQuery] ProductFilterDto filterDto)
		{
			var products = await _productService.GetFilteredProductsAsync(filterDto);

			if (!products.Any())
			{
				return NotFound("No products found matching the criteria.");
			}

			return Ok(products);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] ProductDto productDto)
		{
			var userId = Guid.Parse(User.Identity.Name); 
			await _productService.AddProductAsync(productDto, userId);
			return CreatedAtAction(nameof(GetById), new { id = productDto.Id }, productDto);
		}
		[HttpPut("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Update([FromBody] ProductDto productDto)
		{
			var userId = Guid.Parse(User.Identity.Name);
			await _productService.UpdateProductAsync(productDto, userId);
			return NoContent();
		}

		[HttpDelete("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var userId = Guid.Parse(User.Identity.Name);
			await _productService.DeleteProductAsync(id, userId);
			return NoContent();
		}



	}
}
