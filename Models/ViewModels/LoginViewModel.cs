using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        [Display(Name = "Username or Email")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
