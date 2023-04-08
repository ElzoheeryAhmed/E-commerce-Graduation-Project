using GraduationProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    public class OrderItemEntityTypeConfiguration:IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            //Relationship
            builder.HasOne(b => b.Order)
                .WithMany(a => a.OrderItems)
                .HasForeignKey(c => c.OrderId);


            builder.HasOne(b => b.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId);




            //Key
            builder.HasKey(t => new { t.OrderId, t.ProductId });
        }
    }
}

//Specifications:
/*
    OrderId:
        - foreignkey
        - required
    ProductId:
        - foreignKey
        - required
   
    Quantity:
        - 
   
  Check Entity for each Foreign keys restore navigation property?


 */