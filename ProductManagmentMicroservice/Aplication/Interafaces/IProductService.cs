using ProductManagmentMicroservice.Aplication.Dtos;

namespace ProductManagmentMicroservice.Aplication.Interafaces
{
	public interface IProductService
	{
		Task<IEnumerable<ProductDto>> GetAllProductsAsync();
		Task<ProductDto> GetProductById(Guid id);
		Task AddProductAsync(ProductDto productDto, Guid userId);
		Task UpdateProductAsync(ProductDto productDto, Guid userId);
		Task DeleteProductAsync(Guid productId, Guid userId);
		Task<IEnumerable<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filterDto);
		Task<IEnumerable<ProductDto>> GetProductsByUserIdAsync(Guid userId);
	}
}
