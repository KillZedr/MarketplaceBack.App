using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Application.Marketplace.DAL.EntityTypeConfigurations.Identity
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.ToTable("Users"); // name table => User not ASPNetUser

            
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

          

            builder.Property(u => u.Сountry)
                .HasMaxLength(100);

            builder.Property(u => u.Address)
                .HasMaxLength(500);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(u => u.RegisteredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);


            builder.HasOne(u => u.ActiveCart)
                 .WithOne()
                 .HasForeignKey<Cart>(c => c.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.PaidCarts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
