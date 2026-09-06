using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderToCreateDTO order)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email)?.ToString();
            var result = await _orderService.CreateOrderAsync(order, userEmail!);

            return HandleResult(result);
        }

        [HttpGet("deliveryMethods")]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var result = await _orderService.GetDeliveryMethodsAsync();
            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email)?.ToString();
            var result = await _orderService.GetOrderByIdAsync(id, userEmail!);

            return HandleResult(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email)?.ToString();
            var result = await _orderService.GetAllOrdersAsync(userEmail!);

            return HandleResult(result);
        }
    }
}
