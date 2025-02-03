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
    public class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Identifier);

           
            builder.Property(pi => pi.ProductId)
                .IsRequired();

            builder.Property(pi => pi.ImageUrl)
                .IsRequired()
                .HasMaxLength(500); 


            builder.HasOne(pi => pi.Product) 
                .WithMany(p => p.ProductImages) 
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); 

          
            builder.HasIndex(pi => pi.ProductId);
        }
    }
}
