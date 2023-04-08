﻿using GraduationProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GraduationProject.Configurations.EntityTypeConfigurations
{
    public class OrderEntityTypeConfiguration: IEntityTypeConfiguration<Order>
    {

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //RelationShip
            builder.HasOne(b => b.Customer)
                .WithMany(d => d.Orders)
                .HasForeignKey(e => e.CustomerId);


            //Id
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            //Status
            builder.Property(b => b.Status)
                .HasConversion(new EnumToStringConverter<OrderStatus>());

            builder.Property(b => b.Status)
                .HasColumnType("varchar(15)");

            builder.Property(b => b.Status)
                .HasDefaultValue(OrderStatus.Ordered);

            //OrderDate
            builder.Property(b => b.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            //ReceiptDate
            builder.Property(b => b.ReceiptDate)
                .IsRequired(false);

            //CustomerId
            builder.Property(b => b.CustomerId)
                .IsRequired();



        }
    }
}


//Specifications:
/*
    Id:
        - key
        - autoGenerated

    Status:
        - defaultValue=>Ordered
        - written into database as string
        - string with max 15 characters

    OrderDate:
        - setCurrentDatebydefault

    ReceiptDate:
        - not required
    
    CustomerId
        - required
 
  Check Entity for  Foreign key restore navigation property?

  
 */