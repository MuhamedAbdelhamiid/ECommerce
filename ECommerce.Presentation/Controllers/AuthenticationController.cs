using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.IdentityDTOs;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO userDetails)
        {
            var result = await _authService.LoginAsync(userDetails);
            return HandleResult(result);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO userDetails)
        {
            var result = await _authService.RegisterAsync(userDetails);
            return HandleResult(result);
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var result = await _authService.CheckEmailAsync(email);
            return result;
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _authService.GetUserByEmailAsync(email!);
            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _authService.GetUserAddressAsync(email!);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDTO address)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _authService.UpdateUserAddressAsync(email!, address);
            return HandleResult(result);
        }
    }
}
