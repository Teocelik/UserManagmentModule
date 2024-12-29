using System.ComponentModel.DataAnnotations;

namespace UserManagmentModule.ViewModels
{
    public class UserForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
