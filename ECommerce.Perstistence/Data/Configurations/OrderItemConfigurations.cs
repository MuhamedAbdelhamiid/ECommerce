using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Perstistence.Data.Configurations
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(
                orderItem => orderItem.Product,
                product =>
                {
                    product.Property(orderItem => orderItem.ProductName).HasMaxLength(100);
                    product.Property(orderItem => orderItem.PictureUrl).HasMaxLength(200);
                }
            );

            builder.Property(orderItem => orderItem.Price).HasPrecision(8, 2);
        }
    }
}
