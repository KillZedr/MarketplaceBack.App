using Marketplace.Domain.ECommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Application.Marketplace.DAL.EntityTypeConfigurations.ECommerce
{
    public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Identifier);

           
            builder.HasOne(ci => ci.Cart) 
                .WithMany(c => c.CartItems) 
                .HasForeignKey(ci => ci.CartId) 
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId);

            
            builder.Property(ci => ci.Quantity)
                .IsRequired() 
                .HasDefaultValue(1);



            
        }
    }
}
