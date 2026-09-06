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
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(dl => dl.Price).HasPrecision(8, 2);
            builder.Property(dl => dl.ShortName).HasMaxLength(50);
            builder.Property(dl => dl.Description).HasMaxLength(100);
            builder.Property(dl => dl.DeliveryTime).HasMaxLength(50);
        }
    }
}
