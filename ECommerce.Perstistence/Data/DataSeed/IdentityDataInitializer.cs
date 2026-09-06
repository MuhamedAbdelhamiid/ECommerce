using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Perstistence.Data.DataSeed
{
    public class IdentityDataInitializer : IDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;

        public IdentityDataInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataInitializer> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {
                    var rolesToSeed = new List<IdentityRole>()
                    {
                        new IdentityRole() { Name = "Admin" },
                        new IdentityRole() { Name = "SuperAdmin" },
                    };

                    foreach (var role in rolesToSeed)
                    {
                        await _roleManager.CreateAsync(role);
                    }
                }
                if (!_userManager.Users.Any())
                {
                    var usersToSeed = new List<ApplicationUser>()
                    {
                        new ApplicationUser()
                        {
                            DisplayName = "Mohamed Abdelhamid",
                            Email = "mohamedabdelhamid@gmail.com",
                            PhoneNumber = "1234567890",
                            UserName = "MohamedAbdelhamid",
                        },
                        new ApplicationUser()
                        {
                            DisplayName = "Mostafa Abdelhamid",
                            Email = "mostafaabdelhamid@gmail.com",
                            PhoneNumber = "1234567891",
                            UserName = "MostafaAbdelhamid",
                        },
                    };

                    await _userManager.CreateAsync(usersToSeed[0], "P@ssw0rd");
                    await _userManager.CreateAsync(usersToSeed[1], "P@ssw0rd");

                    await _userManager.AddToRoleAsync(usersToSeed[0], "SuperAdmin");
                    await _userManager.AddToRoleAsync(usersToSeed[1], "Admin");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while seeding identity data: {ex.Message}");
            }
        }
    }
}
