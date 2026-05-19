using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IFacultyService _facultyService;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentService departmentService, IFacultyService facultyService, IMapper mapper = null)
        {
            _departmentService = departmentService;
            _facultyService = facultyService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var faculties = await _facultyService.GetAllFacultiesAsync();
            var model = new DepartmentCreateDto
            {
                Faculties = faculties
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }

            try
            {
                // FacultyId'den FacultyName'i al
                var faculty = await _facultyService.GetFacultyByIdAsync(model.FacultyId);
                if (faculty == null)
                {
                    ModelState.AddModelError("", "Geçersiz fakülte seçildi.");
                    model.Faculties = await _facultyService.GetAllFacultiesAsync();
                    return View(model);
                }

                var departmentDto = new DepartmentListDto
                {
                    DepartmentName = model.DepartmentName,
                    FacultyName = faculty.FacultyName
                };

                await _departmentService.CreateDepartmentAsync(departmentDto);
                TempData["SuccessMessage"] = "Bölüm başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                TempData["ErrorMessage"] = "Bölüm bulunamadı.";
                return RedirectToAction("Index");
            }

            var faculties = await _facultyService.GetAllFacultiesAsync();
            var selectedFaculty = faculties.FirstOrDefault(f => f.FacultyName == department.FacultyName);

            var model = new DepartmentCreateDto
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName,
                FacultyId = selectedFaculty?.Id ?? Guid.Empty,
                FacultyName = department.FacultyName,
                Faculties = faculties
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, DepartmentCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }

            try
            {
                // FacultyId'den FacultyName'i al
                var faculty = await _facultyService.GetFacultyByIdAsync(model.FacultyId);
                if (faculty == null)
                {
                    ModelState.AddModelError("", "Geçersiz fakülte seçildi.");
                    model.Faculties = await _facultyService.GetAllFacultiesAsync();
                    return View(model);
                }

                var departmentDto = new DepartmentListDto
                {
                    Id = id,
                    DepartmentName = model.DepartmentName,
                    FacultyName = faculty.FacultyName
                };

                await _departmentService.UpdateDepartmentAsync(id, departmentDto);
                TempData["SuccessMessage"] = "Bölüm başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                TempData["ErrorMessage"] = "Bölüm bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(department);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
                TempData["SuccessMessage"] = "Bölüm başarıyla silindi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
