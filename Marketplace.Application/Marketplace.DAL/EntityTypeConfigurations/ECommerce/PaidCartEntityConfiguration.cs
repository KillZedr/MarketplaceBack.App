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
    public class PaidCartEntityConfiguration : IEntityTypeConfiguration<PaidCart>
    {
        public void Configure(EntityTypeBuilder<PaidCart> builder)
        {
            builder.HasKey(pc => pc.Identifier);
            


            builder.HasOne(pc => pc.Cart)
                .WithOne(c => c.PaidCart)
                .HasForeignKey<PaidCart>(pc => pc.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.User)
                .WithMany(u => u.PaidCarts)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Свойства
            builder.Property(pc => pc.PaidAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); 

            builder.Property(pc => pc.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(6,2)"); 

            builder.Property(pc => pc.PaymentMethod)
                .IsRequired()
                .HasMaxLength(20); 

            builder.Property(pc => pc.TransactionId)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
