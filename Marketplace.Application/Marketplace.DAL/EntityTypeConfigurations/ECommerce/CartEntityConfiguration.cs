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
    public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Identifier);


            builder.HasOne(c => c.User)
                .WithMany(u => u.PaidCarts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart) 
                .HasForeignKey(ci => ci.CartId) 
                .OnDelete(DeleteBehavior.Cascade); 

       
            builder.Property(c => c.IsPaid)
                .IsRequired() 
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.PaidAt)
                .IsRequired(false);

            builder.HasOne(c => c.PaidCart)
                .WithOne(pc => pc.Cart)  
                .HasForeignKey<PaidCart>(pc => pc.CartId);


            builder.HasIndex(c => c.UserId);

        }
    }
}
