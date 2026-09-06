using Admin.Dashboard.Models.Roles;
using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            return await HandleRoleCreation(model, ModelState);
        }

        public async Task<IActionResult> Delete(string id)
        {
            return await HandleDeleteOperation(id);
        }

        [HttpGet]
        public Task<IActionResult> GetRoleToEdit(string id)
        {
            var role = _roleManager.FindByIdAsync(id);
            if (role is null) { }
        }

        #region Helper Methods
        private async Task<IActionResult> HandleRoleCreation(
            CreateRoleViewModel model,
            ModelStateDictionary modelState
        )
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExists)
                {
                    TempData["Error"] = "Role already exists.";
                    return RedirectToAction(nameof(Index));
                }

                var newRole = new IdentityRole() { Name = model.Name };

                await _roleManager.CreateAsync(newRole);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> HandleDeleteOperation(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is not null)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

                if (usersInRole.Any())
                {
                    TempData["Error"] =
                        "Cannot delete this role, as it is currently assigned to one or more users.";
                }
                else
                    await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
