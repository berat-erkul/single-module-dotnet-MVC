using System.ComponentModel.DataAnnotations;
using FirmaPortal.Entities;

namespace FirmaPortal.ViewModels;

public class UserFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunlu.")]
    [StringLength(100)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunlu.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [StringLength(150)]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 4, ErrorMessage = "Şifre en az 4 karakter.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Password { get; set; }

    [Display(Name = "Rol")]
    public Role Role { get; set; }
}
