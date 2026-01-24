using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Identity;


namespace ProjeHavuzu.MVCUI.Models.AccountModels
{

    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        
         
    }
}
