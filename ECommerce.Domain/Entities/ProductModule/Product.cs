using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.ProductModule
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }

        #region Relationships
        #region Product Has One Type
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; } = default!;
        #endregion

        #region Product Has One Brand
        public ProductBrand ProductBrand { get; set; } = default!;
        public int ProductBrandId { get; set; } = default!;
        #endregion
        #endregion
    }
}
