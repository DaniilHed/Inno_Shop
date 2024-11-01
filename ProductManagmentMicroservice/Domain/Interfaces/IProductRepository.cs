using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Domain.Entities;

namespace ProductManagmentMicroservice.Domain.Interfaces
{
	public interface IProductRepository
	{
		
		
		Task<IEnumerable<Product>> GetAllProductsAsync();
		Task<Product> GetProductByIdAsync(Guid id);
		Task AddProductAsync(Product product);
		Task UpdateProductAsync(Product product);
		Task DeleteProductAsync(Guid id);
		Task<IEnumerable<Product>> GetFilteredProductsAsync(ProductFilterDto filterDto);
		Task<IEnumerable<Product>> GetProductsByUserIdAsync(Guid userId);
		void Save();
	}
}
