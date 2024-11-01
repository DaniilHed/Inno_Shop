namespace ProductManagmentMicroservice.Aplication.Dtos
{
	public class ProductDto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public bool IsAvaiable { get; set; }
		//public Guid UserId { get; set; }
	}
}
