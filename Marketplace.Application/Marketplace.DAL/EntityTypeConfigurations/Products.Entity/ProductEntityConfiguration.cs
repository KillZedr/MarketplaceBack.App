using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Application.Marketplace.DAL.EntityTypeConfigurations.Products.Entity
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Identifier);

          
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50); 

            builder.Property(p => p.Description)
                .HasMaxLength(500); 

            builder.Property(p => p.Price)
                .HasColumnType("decimal(6,2)"); 

            builder.Property(p => p.DiscountPrice)
                .HasColumnType("decimal(6,2)"); 

            builder.Property(p => p.Stock)
                .IsRequired()
                .HasDefaultValue(0); 

            builder.Property(p => p.AverageRating)
                .HasColumnType("float")
                .HasDefaultValue(0.0);

            builder.Property(p => p.ReviewsCount)
                .HasDefaultValue(0); 

            builder.HasOne(p => p.Category) 
                .WithMany(c => c.Products)  
                .HasForeignKey(p => p.CategoryId) 
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(p => p.Manufacturer) 
                .WithMany(m => m.Products) 
                .HasForeignKey(p => p.ManufacturerId) 
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product) 
                .HasForeignKey(pi => pi.ProductId) 
                .OnDelete(DeleteBehavior.Cascade); 


        /*    builder.HasIndex(p => p.Name)
                .IsUnique(); */
        }
    }
}
