
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize]
    public class ProjectSubmissionController : Controller
    {
        private readonly IProjectSubmissionService _submissionService;
        private readonly UserManager<AppUser> _userManager;

        public ProjectSubmissionController(IProjectSubmissionService submissionService, UserManager<AppUser> userManager)
        {
            _submissionService = submissionService;
            _userManager = userManager;
        }

        // ÖĞRENCİ: Yüklenen Projeleri Listele ve Yükleme Ekranı
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));
            var submissions = await _submissionService.GetSubmissionsByStudentIdAsync(userId);
            return View(submissions);
        }

        // ÖĞRENCİ: Yükleme İşlemi (POST)
        [HttpPost]
        [Authorize(Roles = "Student")]
        [RequestSizeLimit(100 * 1024 * 1024)] // 100MB Limit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile projectFile)
        {
            if (projectFile == null || projectFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya seçiniz.";
                return RedirectToAction("Index");
            }

            // Client-side extension check (Backend serviste de tekrar edilecek)
            if (Path.GetExtension(projectFile.FileName).ToLower() != ".zip")
            {
                TempData["ErrorMessage"] = "Sadece .zip uzantılı dosya yükleyebilirsiniz.";
                return RedirectToAction("Index");
            }

            try
            {
                var userId = Guid.Parse(_userManager.GetUserId(User));

                // IFormFile.OpenReadStream() ile stream olarak servise gönderiyoruz (Memory'e tam yüklemeden)
                using (var stream = projectFile.OpenReadStream())
                {
                    await _submissionService.UploadSubmissionAsync(userId, stream, projectFile.FileName);
                }

                TempData["SuccessMessage"] = "Proje dosyası başarıyla yüklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ADMIN: Tüm Gönderimleri Listele
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminList()
        {
            var submissions = await _submissionService.GetAllSubmissionsAsync();

            // StudentId -> FullName eşleştirmesi
            var studentIds = submissions.Select(s => s.StudentId).Distinct().ToList();
            var studentNames = new Dictionary<Guid, string>();
            foreach (var sid in studentIds)
            {
                var user = await _userManager.FindByIdAsync(sid.ToString());
                studentNames[sid] = user != null ? $"{user.FirstName} {user.LastName}" : sid.ToString();
            }
            ViewBag.StudentNames = studentNames;

            return View(submissions);
        }

        // ORTAK: Dosya İndirme
        [Authorize(Roles = "Admin,Teacher,Student")] // Student kendi dosyasını indirebilir mi? Evet.
        public async Task<IActionResult> Download(Guid id, string? returnUrl = null)
        {
            try
            {
                var (fileStream, contentType, fileName) = await _submissionService.GetFileDownloadAsync(id);
                // Dosyayı stream olarak döndür
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Dosya indirilemedi: " + ex.Message;
                return RedirectToSubmissionsList(returnUrl);
            }
        }

        // ADMIN: Silme
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _submissionService.DeleteSubmissionAsync(id);
            TempData["SuccessMessage"] = "Kayıt ve dosya silindi.";
            return RedirectToAction("AdminList");
        }

        // ADMIN: Onaylama
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Approve(Guid id, string? returnUrl = null)
        {
            await _submissionService.ApproveSubmissionAsync(id);
            TempData["SuccessMessage"] = "Proje onaylandı.";
            return RedirectToSubmissionsList(returnUrl);
        }

        // ADMIN: Reddetme
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Reject(Guid id, string? returnUrl = null)
        {
            await _submissionService.RejectSubmissionAsync(id);
            TempData["SuccessMessage"] = "Proje reddedildi.";
            return RedirectToSubmissionsList(returnUrl);
        }

        private IActionResult RedirectToSubmissionsList(string? returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            if (User.IsInRole("Teacher"))
                return RedirectToAction("ProjectSubmissions", "Teacher");

            return RedirectToAction("AdminList");
        }
    }
}
