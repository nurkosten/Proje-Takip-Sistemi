using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Identity;


namespace ProjeHavuzu.MVCUI.Models.AccountModels
{

    public class RegisterViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Ad alanı zorunludur.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Ad")]
        public string FirstName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Soyad")]
        public string LastName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "E-posta")]
        public string Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Şifre")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Şifre Tekrar")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
