using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProjectService _projectService;

        public HomeController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Eski/alternatif URL: proje oluşturma tek kaynak olarak Project/Create üzerinden yürütülür.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Create), "Project");
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint (Home/Ana Sayfa).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetProjectsData()
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

            var result = await _projectService.GetProjectsServerSideAsync(request);

            return Json(new
            {
                draw = result.Draw,
                recordsTotal = result.RecordsTotal,
                recordsFiltered = result.RecordsFiltered,
                data = result.Data.Select(p => new
                {
                    p.Id,
                    projectTitle = p.ProjectTitle,
                    categoryName = p.CategoryName,
                    difficultyLevel = p.DifficultyLevel.ToString(),
                    completionPercentage = p.CompletionPercentage,
                    endDate = p.EndDate.ToString("dd.MM.yyyy"),
                    createdByFullName = p.CreatedByFullName,
                    consultantFullName = p.ConsultantFullName ?? "Danışman Atanmadı",
                    isActive = p.IsActive
                })
            });
        }

        public IActionResult ProjectDetails(Guid id, string? returnUrl = null)
        {
            if (User.IsInRole("Teacher"))
            {
                return RedirectToAction("ProjectDetails", "Teacher", new { id, returnUrl = returnUrl ?? "/Home/Index" });
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Details", "Project", new { id });
            }

            return RedirectToAction("ProjectDetails", "Student", new { id, returnUrl = returnUrl ?? "/Home/Index" });
        }
    }
}
