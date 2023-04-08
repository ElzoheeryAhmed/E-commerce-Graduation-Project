using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Models;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    class CartItemEntityTypeConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            //Relationship
            builder.HasOne(b => b.Customer)
                .WithMany(a => a.CartItems)
                .HasForeignKey(c => c.CustomerId);

            builder.HasOne(b => b.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId);

            //Key
            builder.HasKey(t => new { t.CustomerId, t.ProductId });



        }
    }
}


//Specifications:
/*
    CustomerId:
        - foreignkey
        - required
    ProductId:
        - foreignKey
        - required
    Quantity:
        - 
   
    Check Entity for each Foreign keys restore navigation property?

 */