using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Aplication.Interafaces;
using ProductManagmentMicroservice.Aplication.Mappers;
using ProductManagmentMicroservice.Domain.Entities;
using ProductManagmentMicroservice.Domain.Interfaces;
using ProductManagmentMicroservice.Infrastructure.Exceptions;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ProductManagmentMicroservice.Aplication.Services
{
	public class ProductService:IProductService
	{
		private readonly IProductRepository _productRepository;

        public ProductService (IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
		public  async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
		{
			var products=await _productRepository.GetAllProductsAsync();
			return products.Select(p=>p.ToProductDto());
		}
		public async Task<ProductDto> GetProductById(Guid id)
		{
			var product = await _productRepository.GetProductByIdAsync(id);
			if (product is null)
			{
				throw new NotFoundException($"Product with ID {id} not found.");
			}
			return product.ToProductDto();
		}
		public async Task<IEnumerable<ProductDto>> GetProductsByUserIdAsync(Guid userId)
		{

			var products = await _productRepository.GetProductsByUserIdAsync(userId);
			return products.Select(p => p.ToProductDto());
		}
		public async Task<IEnumerable<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filterDto)
		{
			var products=await _productRepository.GetFilteredProductsAsync(filterDto);
			return products.Select(p => p.ToProductDto());
		}

		public async Task AddProductAsync (ProductDto productDto,Guid userId)
		{
			if(IsExist(productDto, userId).Result)
			{
				throw new BadRequestException($"Product with Name: {productDto.Name} and Description: {productDto.Description} is already exist.");
			}

			var newproduct = new Product
			{
				Id =Guid.NewGuid(),
				Name = productDto.Name,
				UserId = userId,
				Price = productDto.Price,
				Description = productDto.Description,
				IsAvaiable= productDto.IsAvaiable,
				CreatedTime= DateTime.Now

			};
			await _productRepository.AddProductAsync(newproduct);
		}
		public async Task UpdateProductAsync (ProductDto productDto, Guid userId)
		{
			var product = await _productRepository.GetProductByIdAsync(productDto.Id);
			if (product is null)
			{
				throw new NotFoundException($"Product with ID {productDto.Id} not found.");
			}

			if(product.UserId != userId)
			{
				throw new ProductUnauthorizedAccessException("You cannot update this product");
			}
			product.Name = productDto.Name;
			product.Price = productDto.Price;
			product.Description = productDto.Description;
			product.IsAvaiable = productDto.IsAvaiable;

			await _productRepository.UpdateProductAsync(product);
			
		}

		public async Task DeleteProductAsync (Guid productId, Guid userId)
		{
			var product = await _productRepository.GetProductByIdAsync(productId);
			if (product is null)
			{
				throw new NotFoundException($"Product with ID {productId} not found.");
			}
			if (product.UserId != userId)
			{
				throw new ProductUnauthorizedAccessException("You cannot delete this product");
			}
			await _productRepository.DeleteProductAsync(productId);
		}
		private async Task<bool> IsExist(ProductDto productDto, Guid userId)
		{
			ProductFilterDto filterDto = new ProductFilterDto
			{
				Name = productDto.Name,
				UserId = userId,
				Description = productDto.Description
			};
			var products = await _productRepository.GetFilteredProductsAsync(filterDto);
			if (products.Any()) return true;

			return false;
		}

		
	}
}
