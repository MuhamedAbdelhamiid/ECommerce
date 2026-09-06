using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.BasketModule;

namespace ECommerce.Domain.Contracts
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> CreateOrUpdateBasketAsync(
            CustomerBasket basket,
            TimeSpan TTL = default
        );
        Task<bool> DeleteBasketAsync(string basketId);
        Task<CustomerBasket?> GetBasketAsync(string basketId);
    }
}
