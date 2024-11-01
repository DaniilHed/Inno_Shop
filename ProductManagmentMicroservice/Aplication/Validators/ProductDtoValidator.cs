using FluentValidation;
using ProductManagmentMicroservice.Aplication.Dtos;

namespace ProductManagmentMicroservice.Aplication.Validators
{
	public class ProductDtoValidator : AbstractValidator<ProductDto>
	{
		public ProductDtoValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty().WithMessage("Product name is required.")
				.MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

			RuleFor(p => p.Description)
				.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

			RuleFor(p => p.Price)
				.GreaterThan(0).WithMessage("Price must be greater than 0.");

		}
	}
}