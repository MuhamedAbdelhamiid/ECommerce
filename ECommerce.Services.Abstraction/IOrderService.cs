using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(
            OrderToCreateDTO orderToCreate,
            string email
        );

        Task<Result<ICollection<DeliveryMethodsDTO>>> GetDeliveryMethodsAsync();

        Task<Result<ICollection<OrderToReturnDTO>>> GetAllOrdersAsync(string userEmail);

        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid orderId, string userEmail);
    }
}
