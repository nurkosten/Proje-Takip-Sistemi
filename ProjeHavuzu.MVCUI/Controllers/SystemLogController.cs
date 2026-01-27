using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
   // [Authorize(Roles = "Admin")] 
    // Rol kontrolünü şimdilik yorum satırı yapıyorum, projenizdeki rol yapısına göre açabilirsiniz.
    public class SystemLogController : Controller
    {
        private readonly ISystemLogService _systemLogService;

        public SystemLogController(ISystemLogService systemLogService)
        {
            _systemLogService = systemLogService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _systemLogService.GetAllLogsAsync();
            return View(logs);
        }
    }
}
