using System.ComponentModel.DataAnnotations;

namespace ProjeHavuzu.MVCUI.Models.AccountModels

{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email zorunlu")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
