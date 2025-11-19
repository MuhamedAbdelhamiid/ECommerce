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
        public ProductOrderOption sort { get; set; }

        private int _pageIndex = 1;

        public  int pageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value > 0 ? value : 1; }
        }


        private const int _defaultPageSize = 5;
        private const int _maxPageSize = 10;
        private int _pageSize = _defaultPageSize;

        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < 0)
                    _pageSize = _defaultPageSize;
                else if (value >= 10)
                    _pageSize = _maxPageSize;
                else
                    _pageSize = value;
            }
        }

    }
}
