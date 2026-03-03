using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using ProjeHavuzu.Data.Repository.Concrete;
using ProjeHavuzu.MVCUI.Models;
using System.Diagnostics;
using System.Security.AccessControl;

namespace ProjeHavuzu.MVCUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public HomeController(IProjectRepository projectRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
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

            var result = await _projectRepository.GetProjectsServerSideAsync(request);

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
                    isActive = p.IsActive
                })
            });
        }

        public IActionResult ProjectDetails(Guid id)
        {
            if (User.IsInRole("Teacher") || User.IsInRole("Admin"))
            {
                return RedirectToAction("ProjectDetails", "Teacher", new { id = id });
            }

            // Öğrenci ise veya rolü yoksa Student detayına gönder (Student controller Authorize ile login'e zorlayabilir)
            return RedirectToAction("ProjectDetails", "Student", new { id = id });
        }
    }
}
