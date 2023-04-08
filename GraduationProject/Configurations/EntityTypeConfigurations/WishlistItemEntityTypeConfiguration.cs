using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Models;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    class WishlistItemEntityTypeConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            //Relationship
            builder.HasOne(b => b.Customer)
                .WithMany(a => a.WishlistItems)
                .HasForeignKey(c => c.CustomerId);


            builder.HasOne(b => b.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId);




            //Key
            builder.HasKey(t => new { t.CustomerId, t.ProductId, t.Name });

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
    Name:
        - unique with customerId&ProductId
    Quantity:
        - 
   
    Check Entity for each Foreign keys restore navigation property?

 */