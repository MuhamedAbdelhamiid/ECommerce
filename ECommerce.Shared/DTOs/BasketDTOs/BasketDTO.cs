using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.BasketDTOs
{
    public class BasketDTO
    {
        public string Id { get; set; } = default!;
        public ICollection<BasketItemDTO> Items { get; set; } = default!;
    }
}
