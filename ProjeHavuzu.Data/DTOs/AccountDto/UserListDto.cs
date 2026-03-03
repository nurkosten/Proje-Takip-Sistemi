using System;

namespace ProjeHavuzu.Data.DTOs.AccountDto
{
    public class UserListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
