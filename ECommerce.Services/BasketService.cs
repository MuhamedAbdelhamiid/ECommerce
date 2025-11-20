using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Basket_Module;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.BasketDTOs;

namespace ECommerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<BasketDTO?> CreateOrUpdateBasketAsync(BasketDTO basketToCreateOrUpdate)
        {
            if (basketToCreateOrUpdate is null)
                return null!;

            var basket = _mapper.Map<CustomerBasket>(basketToCreateOrUpdate);
            var returnedBasket = await _basketRepository.CreateOrUpdateBasketAsync(
                basket,
                TimeSpan.FromDays(5)
            );

            return _mapper.Map<BasketDTO>(returnedBasket);
        }

        public async Task<bool> DeleteBasketAsync(string basketId) =>
            await _basketRepository.DeleteBasketAsync(basketId);

        public async Task<BasketDTO?> GetBasketAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            return basket is not null ? _mapper.Map<BasketDTO>(basket) : null;
        }
    }
}
