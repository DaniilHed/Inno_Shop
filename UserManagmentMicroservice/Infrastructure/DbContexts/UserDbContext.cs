
using Microsoft.EntityFrameworkCore;
using UserManagmentMicroservice.Domain.Entities;

namespace UserManagmentMicroservice.Infrastructure.DbContexts
{
	public class UserDbContext: DbContext
	{
		

		public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

		public DbSet<UserIdentity> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<UserIdentity>()
				.HasKey(u => u.Id);
			modelBuilder.Entity<UserIdentity>()
				.Property(u => u.Email)
				.IsRequired();
			modelBuilder.Entity<UserIdentity>()
		        .Property(u => u.Role)
		        .HasConversion<string>();
		}

	}
}
