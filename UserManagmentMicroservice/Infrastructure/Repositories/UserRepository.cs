

using Microsoft.EntityFrameworkCore;
using UserManagmentMicroservice.Domain.Entities;
using UserManagmentMicroservice.Domain.Interfaces;
using UserManagmentMicroservice.Infrastructure.DbContexts;

namespace UserManagmentMicroservice.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly UserDbContext _dbContext;

		public UserRepository(UserDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddUserAsync(UserIdentity user)
		{
			await _dbContext.Users.AddAsync(user);
			Save();
		}

		public async Task DeleteUserAsync(Guid id)
		{
			var user = await GetUserByIdAsync(id);
			if (user != null)
			{
				_dbContext.Users.Remove(user);
				Save();
			}
		}

		public async Task<IEnumerable<UserIdentity>> GetAllUsersAsync()
		{
			return await _dbContext.Users.ToListAsync();
		}

		public async Task<UserIdentity> GetUserByIdAsync(Guid id)
		{
			return await _dbContext.Users.FindAsync(id);
		}
		public async Task<UserIdentity> FindUserByEmailAsync(string email)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task UpdateUserAsync(UserIdentity user)
		{
			_dbContext.Update(user);
			Save();
		}
		public async void Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		
	}
}
