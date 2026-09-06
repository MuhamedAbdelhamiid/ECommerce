using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Perstistence.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(
                order => order.ShippingAddress,
                address =>
                {
                    address.Property(address => address.FirstName).HasMaxLength(50);
                    address.Property(address => address.LastName).HasMaxLength(50);
                    address.Property(address => address.Street).HasMaxLength(50);
                    address.Property(address => address.Country).HasMaxLength(50);
                    address.Property(address => address.City).HasMaxLength(50);
                }
            );

            builder.Property(order => order.SubTotal).HasPrecision(8, 2);
        }
    }
}
