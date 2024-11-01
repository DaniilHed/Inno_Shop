

using UserManagmentMicroservice.Domain.Entities;

namespace UserManagmentMicroservice.Domain.Interfaces
{
	public interface IUserRepository
	{
		Task<UserIdentity> GetUserByIdAsync(Guid id);
		Task<UserIdentity> FindUserByEmailAsync(string email);
		Task<IEnumerable<UserIdentity>> GetAllUsersAsync();
		Task AddUserAsync(UserIdentity user);
		Task UpdateUserAsync(UserIdentity user);
		Task DeleteUserAsync(Guid id);

		void Save();
	}
}
