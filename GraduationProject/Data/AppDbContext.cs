using GraduationProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Data
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions contextOptions): base(contextOptions)
		{}

		public DbSet<Product> Products { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Rating> Ratings { get; set; }
		public DbSet<Review> Review { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			
			builder.Entity<Rating>()
				.HasKey(r => new {r.UserId, r.ProductId});
			
			builder.Entity<Rating>()
				.HasOne(r => r.User)
				.WithMany(u => u.Ratings)
				.HasForeignKey(r => r.UserId);
			
			builder.Entity<Rating>()
				.HasOne(r => r.Product)
				.WithMany(p => p.Ratings)
				.HasForeignKey(r => r.ProductId);
			
			
			builder.Entity<Review>()
				.HasKey(r => new {r.UserId, r.ProductId});
			
			builder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId);
			
			builder.Entity<Review>()
				.HasOne(r => r.Product)
				.WithMany(p => p.Reviews)
				.HasForeignKey(r => r.ProductId);
		}
		
		// TODO: Remove this method before deploying the backend.
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
