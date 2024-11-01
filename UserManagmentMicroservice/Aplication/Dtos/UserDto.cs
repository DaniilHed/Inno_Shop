using UserManagmentMicroservice.Domain.Enums;

namespace UserManagmentMicroservice.Aplication.Dtos
{
	public class UserDto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public required string Email { get; set; }

		
		public UserRole Role { get; set; }
	}
}
