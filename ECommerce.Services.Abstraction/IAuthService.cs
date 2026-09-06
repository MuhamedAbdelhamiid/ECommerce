using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.IdentityDTOs;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IAuthService
    {
        Task<Result<UserDTO>> LoginAsync(LoginDTO login);
        Task<Result<UserDTO>> RegisterAsync(RegisterDTO register);
        Task<bool> CheckEmailAsync(string email);
        Task<Result<UserDTO>> GetUserByEmailAsync(string email);
        Task<Result<AddressDTO>> GetUserAddressAsync(string userEmail);
        Task<Result<AddressDTO>> UpdateUserAddressAsync(
            string userEmail,
            AddressDTO updatedAddress
        );
    }
}
