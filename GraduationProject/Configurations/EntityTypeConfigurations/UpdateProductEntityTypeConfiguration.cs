using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Models;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    class UpdateProductEntityTypeConfiguration : IEntityTypeConfiguration<UpdateProduct>
    {
        public void Configure(EntityTypeBuilder<UpdateProduct> builder)
        {
            //Relationship
            builder.HasOne(b => b.CurrentProduct)
                .WithMany()
                .HasForeignKey(d => d.CurrentProductId)
                .OnDelete(DeleteBehavior.NoAction);///////Be careful ya 3am
            builder.HasOne(b => b.ProductUpdate)
                .WithOne()
                .HasForeignKey<UpdateProduct>(b => b.ProductUpdateId)
                .OnDelete(DeleteBehavior.Cascade);



            //key
            builder.HasKey(b => new { b.CurrentProductId, b.ProductUpdateId });
        }
    }
}
