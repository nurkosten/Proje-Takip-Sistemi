using System.ComponentModel.DataAnnotations;

namespace ProjeHavuzu.MVCUI.Models.LoginModels
{
    public class LoginViewModel
    {

        public string UserNameOrEmail { get; set; } = "";

        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }

    }
}