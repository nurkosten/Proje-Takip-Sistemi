using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using ProjeHavuzu.Data.Entites.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnifiedLogService _unifiedLogService;

        public AdminController(UserManager<AppUser> userManager, IUnifiedLogService unifiedLogService)
        {
            _userManager = userManager;
            _unifiedLogService = unifiedLogService;
        }

        public async Task<IActionResult> Academicians()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            return View(teachers);
        }

        /// <summary>
        /// Birleşik Log Yönetimi Sayfası
        /// GET: /Admin/Logs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logs([FromQuery] UnifiedLogFilterDto? filter)
        {
            filter ??= new UnifiedLogFilterDto();
            var viewModel = await _unifiedLogService.GetLogsViewModelAsync(filter);
            return View(viewModel);
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint (Logs).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetLogsData([FromForm] UnifiedLogFilterDto filter)
        {
            try
            {
                var request = new DataTableRequest
                {
                    Draw = int.TryParse(Request.Form["draw"], out var draw) ? draw : 1,
                    Start = int.TryParse(Request.Form["start"], out var start) ? start : 0,
                    Length = int.TryParse(Request.Form["length"], out var length) ? length : 50,
                    Search = new DataTableSearch
                    {
                        Value = Request.Form["search[value]"].ToString()
                    },
                    Columns = new List<DataTableColumn>(),
                    Order = new List<DataTableOrder>()
                };

                var colIdx = 0;
                while (!string.IsNullOrEmpty(Request.Form[$"columns[{colIdx}][data]"]))
                {
                    request.Columns.Add(new DataTableColumn
                    {
                        Data = Request.Form[$"columns[{colIdx}][data]"].ToString(),
                        Name = Request.Form[$"columns[{colIdx}][name]"].ToString(),
                        Searchable = Request.Form[$"columns[{colIdx}][searchable]"] == "true",
                        Orderable = Request.Form[$"columns[{colIdx}][orderable]"] == "true"
                    });
                    colIdx++;
                }

                var orderIdx = 0;
                while (!string.IsNullOrEmpty(Request.Form[$"order[{orderIdx}][column]"]))
                {
                    request.Order.Add(new DataTableOrder
                    {
                        Column = int.TryParse(Request.Form[$"order[{orderIdx}][column]"], out var col) ? col : 0,
                        Dir = Request.Form[$"order[{orderIdx}][dir]"].ToString()
                    });
                    orderIdx++;
                }

                var result = await _unifiedLogService.GetLogsDataAsync(request, filter);

                return Json(new
                {
                    draw = result.Draw,
                    recordsTotal = result.RecordsTotal,
                    recordsFiltered = result.RecordsFiltered,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetLogsData: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Eski logları temizle
        /// POST: /Admin/Logs/ClearOldLogs
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearOldLogs(int olderThanDays = 30)
        {
            var deletedCount = await _unifiedLogService.ClearOldLogsAsync(olderThanDays);
            TempData["SuccessMessage"] = $"{deletedCount} adet {olderThanDays} günden eski log kaydı temizlendi.";
            return RedirectToAction("Logs");
        }
    }
}
