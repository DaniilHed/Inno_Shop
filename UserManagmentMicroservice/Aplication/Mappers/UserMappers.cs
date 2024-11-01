using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Domain.Entities;

namespace UserManagmentMicroservice.Aplication.Mappers
{
	public static class UserMappers
	{
		public static UserDto ToUserDto(this UserIdentity User)
		{
			return new UserDto
			{
				Id = User.Id,
				Name = User.Name,
				Email = User.Email,
				Role = User.Role,
			};
		}
	}
}
