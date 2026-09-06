using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.IdentityDTOs;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<Result<AddressDTO>> GetUserAddressAsync(string userEmail)
        {
            var user = await _userManager
                .Users.Include(user => user.Address)
                .FirstOrDefaultAsync(user => user.Email == userEmail);

            if (user is null)
                return Result<AddressDTO>.Fail(
                    Error.NotFound(
                        code: "User.NotFound",
                        description: $"User With Email: {userEmail} Was Not Found"
                    )
                );

            if (user.Address is null)
                return Result<AddressDTO>.Fail(
                    Error.NotFound(
                        code: "UserAddress.NotFound",
                        description: $"User With Email: {userEmail} His Address Was Not Found"
                    )
                );

            var userAddress = _mapper.Map<AddressDTO>(user.Address);

            return Result<AddressDTO>.Ok(userAddress);
        }

        public async Task<Result<UserDTO>> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<UserDTO>.Fail(
                    Error.NotFound(code: "User.NotFound", description: "User Not Found")
                );

            var userToReturn = new UserDTO(
                user!.DisplayName,
                await GenerateTokenAsync(user),
                email
            );

            return Result<UserDTO>.Ok(userToReturn);
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user is null)
                return Result<UserDTO>.Fail(Error.InvalidCredintals());

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, login.Password);

            if (!passwordCorrect)
                return Result<UserDTO>.Fail(Error.InvalidCredintals());

            var token = await GenerateTokenAsync(user);

            var userToReturn = new UserDTO(user.DisplayName, token, user.Email!);
            return Result<UserDTO>.Ok(userToReturn);
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO register)
        {
            var userToInsert = new ApplicationUser()
            {
                Email = register.Email,
                PhoneNumber = register.PhoneNumber,
                DisplayName = register.DisplayName,
                UserName = register.UserName,
            };

            var returnedIdentityUser = await _userManager.CreateAsync(
                userToInsert,
                register.Password
            );
            if (returnedIdentityUser.Succeeded)
            {
                var token = await GenerateTokenAsync(userToInsert);
                return Result<UserDTO>.Ok(new UserDTO(register.DisplayName, token, register.Email));
            }

            return Result<UserDTO>.Fail(
                returnedIdentityUser
                    .Errors.Select(E => Error.Validation(E.Code, E.Description))
                    .ToList()
            );
        }

        public async Task<Result<AddressDTO>> UpdateUserAddressAsync(
            string userEmail,
            AddressDTO updatedAddress
        )
        {
            var user = await _userManager
                .Users.Include(user => user.Address)
                .FirstOrDefaultAsync(user => user.Email == userEmail);

            if (user is null)
                return Result<AddressDTO>.Fail(
                    Error.NotFound(
                        code: "User.NotFound",
                        description: $"User With Email: {userEmail} Was Not Found"
                    )
                );

            var newAddress = _mapper.Map<Address>(updatedAddress);
            newAddress.UserId = user.Id;
            user.Address = newAddress;

            await _userManager.UpdateAsync(user);

            return Result<AddressDTO>.Ok(updatedAddress);
        }

        #region Helper Methods

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var userCliams = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                userCliams.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration.GetSection("JWTOptions")["SecretKey"]!;
            var issuer = _configuration.GetSection("JWTOptions")["Issuer"]!;
            var audience = _configuration.GetSection("JWTOptions")["Audience"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: userCliams,
                expires: DateTime.UtcNow.AddHours(1),
                audience: audience,
                signingCredentials: signingCred,
                issuer: issuer
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
