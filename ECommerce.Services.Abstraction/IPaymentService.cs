using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketModuleDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IPaymentService
    {
        Task<Result<BasketDTO>> ProcessPaymentAsync(string basketId);
        Task UpdateOrderPaymentStatus(string request, string stripeSignature);
    }
}
