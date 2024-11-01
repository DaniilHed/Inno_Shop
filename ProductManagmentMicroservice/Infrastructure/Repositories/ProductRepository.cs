using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Domain.Entities;
using ProductManagmentMicroservice.Domain.Interfaces;
using ProductManagmentMicroservice.Infrastructure.DbContexts;

namespace ProductManagmentMicroservice.Infrastructure.Repositories
{
	public class ProductRepository :IProductRepository
	{
		private readonly ProductDbContext _dbContext;

		public ProductRepository(ProductDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _dbContext.Products.ToListAsync();
		}

		public async Task<Product> GetProductByIdAsync(Guid id)
		{
			return await _dbContext.Products.FindAsync(id);
		}
		public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(Guid userId)
		{
			
			return await _dbContext.Products.Where(p=> p.UserId == userId).ToListAsync();
		}
		public async Task<IEnumerable<Product>> GetFilteredProductsAsync(ProductFilterDto filterDto)
		{
			IQueryable<Product> productsQuery = _dbContext.Products;


			if (filterDto.UserId.HasValue)
			{
				productsQuery = productsQuery.Where(p => p.UserId==filterDto.UserId.Value);
			}
			if (!string.IsNullOrWhiteSpace(filterDto.Name))
			{
				productsQuery=productsQuery.Where(p=>p.Name.Contains(filterDto.Name));
			}
			if (!string.IsNullOrWhiteSpace(filterDto.Description))
			{
				productsQuery = productsQuery.Where(p => p.Name.Contains(filterDto.Description));
			}
			if (filterDto.MinPrice.HasValue)
			{
				productsQuery = productsQuery.Where(p => p.Price >= filterDto.MinPrice.Value);
			}
			if (filterDto.MaxPrice.HasValue)
			{
				productsQuery = productsQuery.Where(p => p.Price <= filterDto.MaxPrice.Value);
			}
			if (filterDto.MaxPrice.HasValue)
			{
				productsQuery = productsQuery.Where(p => p.IsAvaiable == filterDto.IsAvailable.Value);
			}



			return await productsQuery.ToListAsync();

		}

		public async Task AddProductAsync(Product product)
		{
			await _dbContext.Products.AddAsync(product);
			Save();
		}

		public async Task DeleteProductAsync(Guid id)
		{
			var product = await _dbContext.Products.FindAsync(id);
			if (product != null)
			{
				_dbContext.Products.Remove(product);
				Save();
			}
		}


		public async Task UpdateProductAsync(Product product)
		{
			var dbproduct = await _dbContext.Products.FindAsync(product.UserId);
			if (dbproduct != null)
			{
				_dbContext.Products.Update(dbproduct);
				Save();
			}
		}

		public async void Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		
	}
}
