using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.OrderDTOs
{
    public record OrderToReturnDTO
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; } = default!;
        public ICollection<OrderItemConfirmationDTO> Items { get; set; } = default!;
        public ShippingAddressDTO Address { get; set; } = default!;

        // will be the delivery method is short name
        public string DeliveryMethod { get; set; } = default!;
        public string OrderStatus { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
