using System.Threading.Tasks;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Models;

namespace UserManagmentMicroservice.Aplication.Interfaces
{
	public interface IUserService
	{
		Task<OperationResult<UserDto>> GetUserByIdAsync(Guid id);
		Task<IEnumerable<UserDto>> GetAllUsersAsync();
		Task<OperationResult<bool>> CreateUserAsync(UserDto userDto, string password);
		Task<OperationResult<bool>> UpdateUserAsync(UserDto userDto);
		Task<OperationResult<bool>> DeleteUserAsync(Guid id);
		Task<OperationResult<string>> AuthenticateAsync(LoginRequestDto loginRequest);
		Task<OperationResult<bool>> RegisterUserAsync(UserDto userDto, string password);
		Task<OperationResult<bool>> ConfirmEmailAsync( string token);
		Task<OperationResult<bool>> ForgotPasswordAsync(string email);
		Task<OperationResult<bool>> ResetPasswordAsync(string token, string newPassword);
	}
}
