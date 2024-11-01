using UserManagmentMicroservice.Domain.Enums;

namespace UserManagmentMicroservice.Domain.Entities
{
	public class UserIdentity
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public required string Email { get; set; }
		public UserRole  Role { get; set; }
		public required string PasswordHash { get; set; }
		public bool IsEmailConfirmed { get; set; }
	}
}
