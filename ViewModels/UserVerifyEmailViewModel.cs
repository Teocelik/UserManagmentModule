using System.ComponentModel.DataAnnotations;

namespace UserManagmentModule.ViewModels
{
    public class UserVerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}