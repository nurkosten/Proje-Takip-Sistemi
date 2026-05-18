using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemLogController : Controller
    {
        private readonly ISystemLogService _systemLogService;
        private readonly UserManager<AppUser> _userManager;

        public SystemLogController(ISystemLogService systemLogService, UserManager<AppUser> userManager)
        {
            _systemLogService = systemLogService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _systemLogService.GetAllLogsAsync();
            return View(logs);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserLogs(Guid userId)
        {
            var logs = await _systemLogService.GetLogsByUserIdAsync(userId);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            ViewBag.UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Bilinmeyen Kullanıcı";
            ViewBag.UserEmail = user != null ? user.Email : "";

            return View(logs);
        }
    }
}
