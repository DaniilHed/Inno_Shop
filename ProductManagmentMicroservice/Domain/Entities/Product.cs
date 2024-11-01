using System.ComponentModel.DataAnnotations;

namespace ProductManagmentMicroservice.Domain.Entities
{
	public class Product
	{
		public  Guid Id { get; set; }
		[Required]
		public required string Name { get; set; } = string.Empty;
		[Required]
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public bool IsAvaiable { get; set; }
		public Guid UserId { get; set; }
		public DateTime CreatedTime { get; set; }

		
	}

	
}
