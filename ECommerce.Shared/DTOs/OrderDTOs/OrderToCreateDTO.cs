using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.OrderDTOs
{
    public record OrderToCreateDTO
    {
        // init => after first intailization it will not be able to set again
        public string BasketId { get; set; } = default!;
        public int DeliveryMethodId { get; set; }
        public ShippingAddressDTO ShipToAddress { get; set; } = default!;
    }
}
