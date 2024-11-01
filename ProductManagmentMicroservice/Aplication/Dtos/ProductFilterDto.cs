namespace ProductManagmentMicroservice.Aplication.Dtos
{
	public class ProductFilterDto
	{
		public string? Name { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public string? Description { get; set; }
		public bool? IsAvailable { get; set; }
		public Guid? UserId { get; set; }
	}
}
