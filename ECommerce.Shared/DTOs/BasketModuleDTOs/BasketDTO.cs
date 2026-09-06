using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.BasketModuleDTOs
{
    public class BasketDTO
    {
        public string Id { get; set; } = default!;
        public IEnumerable<BasketItemDTO> Items { get; set; } = default!;
        public decimal ShippingPrice { get; set; }
        public int? DeliveryMethodId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
