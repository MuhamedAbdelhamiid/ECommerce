using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketModuleDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IBasketService
    {
        Task<Result<BasketDTO?>> CreateOrUpdateBasketAsync(BasketDTO basketDTO);
        Task<bool> DeleteBasketAsync(string basketId);
        Task<Result<BasketDTO?>> GetBasketByIdAsync(string basketId);
    }
}
