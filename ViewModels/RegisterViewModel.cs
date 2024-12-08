using System.ComponentModel.DataAnnotations;

namespace UserManagmentModule.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adını giriniz!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-mail adresini giriniz!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "şifreyi giriniz!")]
        [MinLength(6, ErrorMessage = "Parola 6 karakterden kısa olmamalıdır!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
        ErrorMessage = "Prolanız, küçük harf, büyük harf ve sayı içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "şifreyi tekrar giriniz!")]
        [Compare("Password", ErrorMessage = "Şifre eşleşmedi!.")]
        public string ConfirmPassword { get; set; }
    }
}
