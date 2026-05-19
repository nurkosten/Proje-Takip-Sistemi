using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.Repository.Abstract;
using System;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        private readonly IMapper _mapper;

        public FacultyController(IFacultyService facultyService, IMapper mapper = null)
        {
            _facultyService = facultyService;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var faculties = await _facultyService.GetAllFacultiesAsync();
            return View("Index", faculties);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacultyCreateDto facultyCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(facultyCreateDto);
            }

            try
            {
                await _facultyService.CreateFacultyAsync(facultyCreateDto);
                TempData["SuccessMessage"] = "Fakülte başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(facultyCreateDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var faculty = await _facultyService.GetFacultyByIdAsync(id);
            if (faculty == null)
            {
                TempData["ErrorMessage"] = "Fakülte bulunamadı.";
                return RedirectToAction("Index");
            }

            var editDto = new FacultyCreateDto
            {
                FacultyName = faculty.FacultyName
            };

            ViewBag.FacultyId = id;
            return View(editDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FacultyCreateDto facultyCreateDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FacultyId = id;
                return View(facultyCreateDto);
            }

            try
            {
                await _facultyService.UpdateFacultyAsync(id, facultyCreateDto);
                TempData["SuccessMessage"] = "Fakülte başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.FacultyId = id;
                return View(facultyCreateDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var faculty = await _facultyService.GetFacultyByIdAsync(id);
            if (faculty == null)
            {
                TempData["ErrorMessage"] = "Fakülte bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(faculty);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _facultyService.DeleteFacultyAsync(id);
                TempData["SuccessMessage"] = "Fakülte başarıyla silindi.";
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
