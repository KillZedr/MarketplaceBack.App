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
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Identifier);

            
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200); 

           
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category) 
                .HasForeignKey(p => p.CategoryId) 
                .OnDelete(DeleteBehavior.Cascade); 

           
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
