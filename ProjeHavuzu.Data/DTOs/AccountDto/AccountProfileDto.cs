using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.AccountDto
{
    public class AccountProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? StudentNumber { get; set; }
    }
}
