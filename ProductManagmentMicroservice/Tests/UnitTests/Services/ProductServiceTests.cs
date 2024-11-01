using Moq;
using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Aplication.Services;
using ProductManagmentMicroservice.Domain.Entities;
using ProductManagmentMicroservice.Domain.Interfaces;
using ProductManagmentMicroservice.Infrastructure.Exceptions;
using Xunit;

namespace ProductManagmentMicroservice.Tests.UnitTests.Services
{
	public class ProductServiceTests
	{
		private readonly Mock<IProductRepository> _productRepositoryMock;
		private readonly ProductService _productService;

		public ProductServiceTests()
		{
			_productRepositoryMock = new Mock<IProductRepository>();
			_productService = new ProductService(_productRepositoryMock.Object);
		}

		[Fact]
		public async Task GetAllProductsAsync_ShouldReturnProducts()
		{
			// Arrange
			var products = new List<Product>
		{
			new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true },
			new Product { Id = Guid.NewGuid(), Name = "Product 2", Description = "Description 2", Price = 20, IsAvaiable = false }
		};

			_productRepositoryMock.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(products);

			// Act
			var result = await _productService.GetAllProductsAsync();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var product = new Product { Id = productId, Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

			// Act
			var result = await _productService.GetProductById(productId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(productId, result.Id);
			Assert.Equal("Product 1", result.Name);
		}

		[Fact]
		public async Task GetProductById_ShouldThrowNotFoundException_WhenProductDoesNotExist()
		{
			// Arrange
			var productId = Guid.NewGuid();

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.GetProductById(productId));
		}

		[Fact]
		public async Task GetProductsByUserIdAsync_ShouldReturnProducts_WhenProductsExist()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var products = new List<Product>
		{
			new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true, UserId = userId },
			new Product { Id = Guid.NewGuid(), Name = "Product 2", Description = "Description 2", Price = 20, IsAvaiable = false, UserId = userId }
		};

			_productRepositoryMock.Setup(repo => repo.GetProductsByUserIdAsync(userId)).ReturnsAsync(products);

			// Act
			var result = await _productService.GetProductsByUserIdAsync(userId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public async Task AddProductAsync_ShouldThrowBadRequestException_WhenProductExists()
		{
			// Arrange
			var productDto = new ProductDto { Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };
			var userId = Guid.NewGuid();
			var existingProduct = new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true, UserId = userId };

			_productRepositoryMock.Setup(repo => repo.GetFilteredProductsAsync(It.IsAny<ProductFilterDto>())).ReturnsAsync(new List<Product> { existingProduct });

			// Act & Assert
			await Assert.ThrowsAsync<BadRequestException>(async () => await _productService.AddProductAsync(productDto, userId));
		}

		[Fact]
		public async Task AddProductAsync_ShouldAddProduct_WhenProductDoesNotExist()
		{
			// Arrange
			var productDto = new ProductDto { Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };
			var userId = Guid.NewGuid();

			_productRepositoryMock.Setup(repo => repo.GetFilteredProductsAsync(It.IsAny<ProductFilterDto>())).ReturnsAsync(new List<Product>());

			// Act
			await _productService.AddProductAsync(productDto, userId);

			// Assert
			_productRepositoryMock.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Once);
		}

		[Fact]
		public async Task UpdateProductAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
		{
			// Arrange
			var productDto = new ProductDto { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };
			var userId = Guid.NewGuid();

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productDto.Id)).ReturnsAsync((Product)null);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.UpdateProductAsync(productDto, userId));
		}

		[Fact]
		public async Task UpdateProductAsync_ShouldThrowProductUnauthorizedAccessException_WhenUserDoesNotOwnProduct()
		{
			// Arrange
			var productDto = new ProductDto { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };
			var userId = Guid.NewGuid();
			var product = new Product { Name = "Product 1", Id = productDto.Id, UserId = Guid.NewGuid() }; // Другой пользователь

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productDto.Id)).ReturnsAsync(product);

			// Act & Assert
			await Assert.ThrowsAsync<ProductUnauthorizedAccessException>(async () => await _productService.UpdateProductAsync(productDto, userId));
		}

		[Fact]
		public async Task UpdateProductAsync_ShouldUpdateProduct_WhenUserOwnsProduct()
		{
			// Arrange
			var productDto = new ProductDto { Id = Guid.NewGuid(), Name = "Product 1", Description = "Description 1", Price = 10, IsAvaiable = true };
			var userId = productDto.Id; // Тот же пользователь
			var product = new Product { Id = productDto.Id, UserId = userId, Name = "Old Name" };

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productDto.Id)).ReturnsAsync(product);

			// Act
			await _productService.UpdateProductAsync(productDto, userId);

			// Assert
			Assert.Equal(productDto.Name, product.Name);
			_productRepositoryMock.Verify(repo => repo.UpdateProductAsync(product), Times.Once);
		}

		[Fact]
		public async Task DeleteProductAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.DeleteProductAsync(productId, userId));
		}

		[Fact]
		public async Task DeleteProductAsync_ShouldThrowProductUnauthorizedAccessException_WhenUserDoesNotOwnProduct()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var product = new Product {Name = "Product 1", Id = productId, UserId = Guid.NewGuid() }; // Другой пользователь

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

			// Act & Assert
			await Assert.ThrowsAsync<ProductUnauthorizedAccessException>(async () => await _productService.DeleteProductAsync(productId, userId));
		}

		[Fact]
		public async Task DeleteProductAsync_ShouldDeleteProduct_WhenUserOwnsProduct()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var userId = productId; // Тот же пользователь
			var product = new Product { Name = "Product 1", Id = productId, UserId = userId };

			_productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

			// Act
			await _productService.DeleteProductAsync(productId, userId);

			// Assert
			_productRepositoryMock.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
		}
	}
}
