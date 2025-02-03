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
    public class ManufacturerEntityConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.HasKey(m => m.Identifier);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200); 

            builder.Property(m => m.Country)
                .HasMaxLength(100); 

            builder.Property(m => m.Website)
                .HasMaxLength(500); 

            
            builder.HasMany(m => m.Products)
                .WithOne(p => p.Manufacturer) 
                .HasForeignKey(p => p.ManufacturerId) 
                .OnDelete(DeleteBehavior.Cascade); 

          
            builder.HasIndex(m => m.Name)
                .IsUnique(); 
        }
    }
}
