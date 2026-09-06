using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using StackExchange.Redis;

namespace ECommerce.Perstistence.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(
            CustomerBasket basket,
            TimeSpan TTL
        )
        {
            var jsonObject = JsonSerializer.Serialize(basket);
            var createdOrUpdated = await _database.StringSetAsync(
                basket.Id,
                jsonObject,
                TTL == default ? TimeSpan.FromDays(7) : TTL
            );

            return await GetBasketAsync(basket.Id);
        }

        public async Task<bool> DeleteBasketAsync(string basketId) =>
            await _database.KeyDeleteAsync(basketId);

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basketToReturn = await _database.StringGetAsync(basketId);
            if (!basketToReturn.IsNullOrEmpty)
            {
                var basketJsonObjectToReturn = JsonSerializer.Deserialize<CustomerBasket>(
                    basketToReturn!
                );
                return basketJsonObjectToReturn;
            }
            return null;
        }
    }
}
