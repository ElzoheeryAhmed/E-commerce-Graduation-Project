using GraduationProject.Configurations.EntityTypeConfigurations;
using GraduationProject.Models;
using GraduationProject.Models.ModelEnums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GraduationProject.Data
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions contextOptions): base(contextOptions)
		{}

		public DbSet<User> Users { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductCategory> ProductCategories { get; set; }
		public DbSet<ProductCategoryJoin> ProductCategoryJoins { get; set; }
		public DbSet<Brand> Brands { get; set; }
		public DbSet<ProductUpdate> ProductUpdates { get; set; }
		
		public DbSet<Rating> Ratings { get; set; }
		public DbSet<Review> Review { get; set; }
		
		public DbSet<Issue> Issues { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<WishlistItem> WishlistItems { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			
			builder.Entity<Product>()
				.Property(p => p.DateAdded)
				.HasDefaultValueSql("getdate()");
			
			builder.Entity<Product>()
				.Property(p => p.Discount)
				.HasColumnType("decimal(18,2)");
			
			builder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");
			
			builder.Entity<Product>()
				.Property(p => p.Status).HasConversion(new EnumToStringConverter<ProductStatus>());
			
            builder.Entity<Product>()
				.Property(p => p.Status)
                .HasColumnType("varchar(15)");
			
            builder.Entity<Product>()
				.Property(p => p.Status)
                .HasDefaultValue(ProductStatus.Added);
			
			builder.Entity<Product>()
				.HasOne(p => p.Brand)
				.WithMany(pb => pb.Products)
				.HasForeignKey(p => p.BrandId);
			
			builder.Entity<Product>()
				.HasMany(p => p.ProductCategories)
				.WithMany(pc => pc.Products)
				.UsingEntity<ProductCategoryJoin>(
					l => l.HasOne<ProductCategory>().WithMany().HasForeignKey(p => p.ProductCategoryId).HasPrincipalKey(p => p.Id),
					r => r.HasOne<Product>().WithMany().HasForeignKey(pc => pc.ProductId).HasPrincipalKey(p => p.Id),
					j => j.HasKey("ProductId", "ProductCategoryId")
				);
			
			builder.Entity<ProductCategory>()
				.Property(pc => pc.Id)
				.UseIdentityColumn(seed: 0, increment: 1);
			
			builder.Entity<Brand>()
				.Property(b => b.Id)
				.UseIdentityColumn(seed: 0, increment: 1);
			
			new ProductUpdateEntityTypeConfiguration().Configure(builder.Entity<ProductUpdate>());
			
			
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
			
			builder.Entity<Rating>()
				.Property(r => r.Timestamp)
				.HasDefaultValueSql("getdate()");
			
			
			builder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId);
			
			builder.Entity<Review>()
				.HasOne(r => r.Product)
				.WithMany(p => p.Reviews)
				.HasForeignKey(r => r.ProductId);
			
			builder.Entity<Review>()
				.Property(r => r.Id)
				.ValueGeneratedOnAdd();
			
			builder.Entity<Review>()
				.Property(r => r.Timestamp)
				.HasDefaultValueSql("getdate()");
			
			
			new IssueEntityTypeConfiguration().Configure(builder.Entity<Issue>());
			new OrderEntityTypeConfiguration().Configure(builder.Entity<Order>());
			new OrderItemEntityTypeConfiguration().Configure(builder.Entity<OrderItem>());
			new CartItemEntityTypeConfiguration().Configure(builder.Entity<CartItem>());
			new WishlistItemEntityTypeConfiguration().Configure(builder.Entity<WishlistItem>());
		}
		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// TODO: Remove this method before deploying the backend.
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
