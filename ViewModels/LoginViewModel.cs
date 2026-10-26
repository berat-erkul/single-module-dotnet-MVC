using System.ComponentModel.DataAnnotations;

namespace FirmaPortal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta zorunlu.")]
    [EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunlu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
