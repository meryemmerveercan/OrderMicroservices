using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderProduct> OrderProducts { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<CustomerAddress> CustomerAddresses { get; set; }
		public DbSet<Product> Products { get; set; }

	}
}
