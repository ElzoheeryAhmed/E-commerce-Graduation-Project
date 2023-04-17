using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Models;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    class ProductUpdateEntityTypeConfiguration : IEntityTypeConfiguration<ProductUpdate>
    {
        public void Configure(EntityTypeBuilder<ProductUpdate> builder)
        {
            //Relationship
            builder.HasOne(b => b.CurrentProduct)
                .WithMany()
                .HasForeignKey(d => d.CurrentProductId)
                .OnDelete(DeleteBehavior.NoAction);///////Be careful ya 3am
            builder.HasOne(b => b.UpdatedProduct)
                .WithOne()
                .HasForeignKey<ProductUpdate>(b => b.UpdatedProductId)
                .OnDelete(DeleteBehavior.Cascade);



            //key
            builder.HasKey(b => new { b.CurrentProductId, b.UpdatedProductId });
        }
    }
}
