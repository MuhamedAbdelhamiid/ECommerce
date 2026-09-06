using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.BasketModuleDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class BasketsController : ApiBaseController
    {
        private readonly IBasketService _basketService;

        public BasketsController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpPost()]
        public async Task<ActionResult<BasketDTO?>> CreateOrUpdateBasket(
            [FromBody] BasketDTO? basketDTO
        )
        {
            var result = await _basketService.CreateOrUpdateBasketAsync(basketDTO!);
            return HandleResult(result);
        }

        [HttpGet()]
        public async Task<ActionResult<BasketDTO?>> GetBasketById(string basketId)
        {
            var result = await _basketService.GetBasketByIdAsync(basketId);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var result = await _basketService.DeleteBasketAsync(id);
            return result;
        }
    }
}
