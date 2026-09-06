using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.Models.Roles
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(256, ErrorMessage = "Role name cannot exceed 256 characters.")]
        public string Name { get; set; } = default!;
    }
}
