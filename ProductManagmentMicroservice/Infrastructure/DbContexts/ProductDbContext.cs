using Microsoft.EntityFrameworkCore;
using ProductManagmentMicroservice.Domain.Entities;

namespace ProductManagmentMicroservice.Infrastructure.DbContexts
{
	public class ProductDbContext:DbContext
	{
		public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) 
		{

		}
		public DbSet<Product> Products { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
