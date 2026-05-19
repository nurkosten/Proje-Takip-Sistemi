using System;
using System.Collections.Generic;

namespace ProjeHavuzu.MVCUI.Models
{
    public class AdminAssignRoleViewModel
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> CurrentRoles { get; set; } = new();
        public string SelectedRole { get; set; } = string.Empty;
        public List<string> AvailableRoles { get; set; } = new();
    }
}
