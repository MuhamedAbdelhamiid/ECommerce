using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketModuleDTOs;

namespace ECommerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepo, IMapper mapper)
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
        }

        public async Task<Result<BasketDTO?>> CreateOrUpdateBasketAsync(BasketDTO basketDTO)
        {
            var basketToCreateOrUpdate = _mapper.Map<CustomerBasket>(basketDTO);
            var returnedBasket = await _basketRepo.CreateOrUpdateBasketAsync(
                basketToCreateOrUpdate
            );

            return MakeDecisionForReturnedBasket(returnedBasket);
        }

        public async Task<bool> DeleteBasketAsync(string basketId) =>
            await _basketRepo.DeleteBasketAsync(basketId);

        public async Task<Result<BasketDTO?>> GetBasketByIdAsync(string basketId)
        {
            var returnedBasket = await _basketRepo.GetBasketAsync(basketId);

            return MakeDecisionForReturnedBasket(returnedBasket);
        }

        #region Helper Methods
        private Result<BasketDTO?> MakeDecisionForReturnedBasket(CustomerBasket? returnedBasket)
        {
            if (returnedBasket is null)
                return Result<BasketDTO?>.Fail(
                    Error.NotFound(description: "The required basket does not exist")
                );

            var basketToReturn = _mapper.Map<BasketDTO?>(returnedBasket);
            return Result<BasketDTO?>.Ok(basketToReturn);
        }

        #endregion
    }
}
