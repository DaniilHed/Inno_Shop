using ProductManagmentMicroservice.Aplication.Dtos;
using ProductManagmentMicroservice.Domain.Entities;

namespace ProductManagmentMicroservice.Aplication.Mappers
{
	public static class ProductMappers
	{
		public static ProductDto ToProductDto(this Product productModel)
		{
			return new ProductDto
			{
				Id = productModel.Id,
				Name = productModel.Name,
				Description = productModel.Description,
				Price = productModel.Price,
				IsAvaiable = productModel.IsAvaiable
				//UserId = productModel.UserId
			};
		}
	}
}
