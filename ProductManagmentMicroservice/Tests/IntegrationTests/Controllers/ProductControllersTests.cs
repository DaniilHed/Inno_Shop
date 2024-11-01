using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagmentMicroservice.API;
using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Aplication.Interafaces;
using System.Security.Claims;
using Xunit;

namespace ProductManagmentMicroservice.Tests.IntegrationTests.Controllers
{
	public class ProductControllersTests
	{
		private readonly Mock<IProductService> _productServiceMock;
		private readonly ProductController _productController;

		public ProductControllersTests()
		{
			_productServiceMock = new Mock<IProductService>();
			_productController = new ProductController(_productServiceMock.Object);
		}

		[Fact]
		public async Task GetAll_ReturnsOk_WhenProductsExist()
		{
		
			var products = new List<ProductDto>
		{
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 1" },
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 2" }
		};

			_productServiceMock.Setup(ps => ps.GetAllProductsAsync())
				.ReturnsAsync(products);

			
			var result = await _productController.GetAll();

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
			Assert.Equal(products.Count, returnProducts.Count);
		}

		[Fact]
		public async Task GetById_ReturnsOk_WhenProductExists()
		{
			
			var productId = Guid.NewGuid();
			var product = new ProductDto { Id = productId, Name = "Product 1" };

			_productServiceMock.Setup(ps => ps.GetProductById(productId))
				.ReturnsAsync(product);

			
			var result = await _productController.GetById(productId);

			
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnProduct = Assert.IsType<ProductDto>(okResult.Value);
			Assert.Equal(productId, returnProduct.Id);
		}

		[Fact]
		public async Task GetProductsByUserId_ReturnsOk_WhenProductsExistForUser()
		{
			
			var userId = Guid.NewGuid();
			var products = new List<ProductDto>
		{
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 1"  },
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 2" }
		};

			_productServiceMock.Setup(ps => ps.GetProductsByUserIdAsync(userId))
				.ReturnsAsync(products);

			
			var result = await _productController.GetProductsByUserId(userId);

			
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
			Assert.Equal(products.Count, returnProducts.Count);
		}

		[Fact]
		public async Task GetProductsByUserId_ReturnsNotFound_WhenNoProductsExistForUser()
		{
			
			var userId = Guid.NewGuid();
			var products = new List<ProductDto>();

			_productServiceMock.Setup(ps => ps.GetProductsByUserIdAsync(userId))
				.ReturnsAsync(products);

			
			var result = await _productController.GetProductsByUserId(userId);

			
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("No products found for this user.", notFoundResult.Value);
		}

		[Fact]
		public async Task GetFilteredProducts_ReturnsOk_WhenProductsExist()
		{
			var filterDto = new ProductFilterDto { Name = "Product 1" };
			var products = new List<ProductDto>
		{
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 1" },
			new ProductDto { Id = Guid.NewGuid(), Name = "Product 2" }
		};

			_productServiceMock.Setup(ps => ps.GetFilteredProductsAsync(filterDto))
				.ReturnsAsync(products);

			
			var result = await _productController.GetFilteredProducts(filterDto);

			
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
			Assert.Equal(products.Count, returnProducts.Count);
		}

		[Fact]
		public async Task GetFilteredProducts_ReturnsNotFound_WhenNoProductsMatchCriteria()
		{
			
			var filterDto = new ProductFilterDto { Name = "Non-existent Product" };
			var products = new List<ProductDto>();

			_productServiceMock.Setup(ps => ps.GetFilteredProductsAsync(filterDto))
				.ReturnsAsync(products);

			
			var result = await _productController.GetFilteredProducts(filterDto);

			
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("No products found matching the criteria.", notFoundResult.Value);
		}

		[Fact]
		public async Task Create_ReturnsCreatedAtAction_WhenProductIsCreatedSuccessfully()
		{
			
			var productDto = new ProductDto { Name = "New Product", Price = 10.0M };
			var userId = Guid.NewGuid(); 
			var productId = Guid.NewGuid(); 
			productDto.Id = productId;

			_productController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString())
					}))
				}
			};

			_productServiceMock.Setup(ps => ps.AddProductAsync(productDto, userId))
				.Returns(Task.CompletedTask);

			
			var result = await _productController.Create(productDto);

			
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			Assert.Equal(nameof(_productController.GetById), createdResult.ActionName);
			Assert.Equal(productId, createdResult.RouteValues["id"]);
		}

		[Fact]
		public async Task Update_ReturnsNoContent_WhenProductIsUpdatedSuccessfully()
		{
			// Arrange
			var productDto = new ProductDto { Id = Guid.NewGuid(), Name = "Updated Product", Price = 20.0M };
			var userId = Guid.NewGuid(); // Simulated user ID

			_productController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString())
					}))
				}
			};

			_productServiceMock.Setup(ps => ps.UpdateProductAsync(productDto, userId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _productController.Update(productDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task Delete_ReturnsNoContent_WhenProductIsDeletedSuccessfully()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var userId = Guid.NewGuid(); // Simulated user ID

			_productController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString())
					}))
				}
			};

			_productServiceMock.Setup(ps => ps.DeleteProductAsync(productId, userId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _productController.Delete(productId);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
