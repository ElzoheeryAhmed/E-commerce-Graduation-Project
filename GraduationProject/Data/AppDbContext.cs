using System.Reflection.Emit;
using GraduationProject.Configurations.EntityTypeConfigurations;
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

		public DbSet<Issue> Issues { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<WishlistItem> WishlistItems { get; set; }
		public DbSet<UpdateProduct> ProdutUpdates { get; set; }

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
			
			// Auto generating the Id on insertion.
			builder.Entity<Review>()
				.Property(r => r.Id)
				.ValueGeneratedOnAdd();

		    new IssueEntityTypeConfiguration().Configure(builder.Entity<Issue>());
		    new OrderEntityTypeConfiguration().Configure(builder.Entity<Order>());
		    new OrderItemEntityTypeConfiguration().Configure(builder.Entity<OrderItem>());
		    new CartItemEntityTypeConfiguration().Configure(builder.Entity<CartItem>());
		    new WishlistItemEntityTypeConfiguration().Configure(builder.Entity<WishlistItem>());
		    new UpdateProductEntityTypeConfiguration().Configure(builder.Entity<UpdateProduct>());
		}

		// TODO: Remove this method before deploying the backend.
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
