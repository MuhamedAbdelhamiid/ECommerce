using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared
{
    public class ProductQueryParams
    {
        public int? typeId { get; set; }
        public int? brandId { get; set; }
        public string? search { get; set; }
    }
}
