using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Domain.Entities;

namespace UserManagmentMicroservice.Aplication.Mappers
{
	public static class LoginRequestMappers
	{
		public static LoginRequestDto ToLoginRequestDto(this UserIdentity User)
		{
			return new LoginRequestDto
			{

				Email = User.Email,
				Password = User.PasswordHash,
			};
		}
	}
}
