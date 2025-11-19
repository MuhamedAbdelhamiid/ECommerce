using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Data.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            #region Product - Brand

            builder.HasOne(P => P.ProductBrand).WithMany().HasForeignKey(p => p.ProductBrandId);
            #endregion

            #region Product - Type

            builder.HasOne(P => P.ProductType).WithMany().HasForeignKey(p => p.ProductTypeId);
            #endregion

            builder.Property(p => p.Name).HasColumnType("nvarchar(100)");

            builder.Property(p => p.PictureUrl).HasMaxLength(200);

            builder.Property(p => p.Description).HasColumnType("nvarchar(500)");

            builder.Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
