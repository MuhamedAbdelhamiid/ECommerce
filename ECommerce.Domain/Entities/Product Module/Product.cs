using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.ProductModule
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }

        #region Relationships

        #region Brand
        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; } = null!;

        #endregion

        #region Type
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; } = null!;
        #endregion

        #endregion
    }
}
