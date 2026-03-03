using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.MailService;
using ProjeHavuzu.Data.MailServices;
using ProjeHavuzu.MVCUI.Models.AccountModels;
using ProjeHavuzu.MVCUI.Models.LoginModels;
using System.Security.Claims;
using ForgotPasswordViewModel = ProjeHavuzu.MVCUI.Models.AccountModels.ForgotPasswordViewModel;


namespace ProjeHavuzu.MVCUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMailSender _emailSender;
        private readonly IBackgroundEmailService _backgroundEmailService;
        private readonly IFacultyService _facultyService;
        private readonly IDepartmentService _departmentService;
        private readonly ApplicationContext _context;

        public AccountController(
            UserManager<AppUser> userManager,
            IMailSender emailSender,
            IBackgroundEmailService backgroundEmailService,
            SignInManager<AppUser> signInManager = null,
            RoleManager<AppRole> roleManager = null,
            ApplicationContext context = null,
            IFacultyService facultyService = null,
            IDepartmentService departmentService = null)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _backgroundEmailService = backgroundEmailService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _facultyService = facultyService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Güvenlik için kullanıcı yoksa bile aynı mesaj
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token, email = model.Email },
                Request.Scheme);

            // Mail gönderimini Hangfire ile arka plana at
            // Kullanıcı bekletilmeden mail kuyruğa eklenir
            _backgroundEmailService.EnqueuePasswordReset(model.Email, resetLink);

            return RedirectToAction("ForgotPasswordConfirmation");


        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);

            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }




        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Domain Kontrolü: Sadece okul mailleri
            if (!model.Email.Trim().EndsWith("ozal.edu.tr", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Sadece kurumsal üniversite e-postanız ile (@ozal.edu.tr vb.) kayıt olabilirsiniz.");
                return View(model);
            }

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName

            };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                // Varsayılan olarak Student rolü ata
                // Rolün varlığını kontrol etmek iyi bir pratik olsa da, SeedData ile oluşturulduğunu varsayıyoruz
                await _userManager.AddToRoleAsync(user, "Student");

                // Email Doğrulama Maili Gönder - Hangfire ile arka planda
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = user.Email }, Request.Scheme);

                // Mail gönderimini Hangfire ile arka plana at
                // Kullanıcı bekletilmeden mail kuyruğa eklenir
                _backgroundEmailService.EnqueueEmailConfirmation(user.Email, confirmationLink);

                TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen e-postanıza gönderilen doğrulama linkine tıklayınız.";

                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Hesabınız başarıyla doğrulandı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login", "Account");
            }

            TempData["ErrorMessage"] = "Email doğrulama başarısız oldu.";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Önce email ile kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Giriş yapabilmek için lütfen E-posta adresinizi doğrulayınız. Mail kutunuzu (Spam dahil) kontrol ediniz.");
                return View(model);
            }

            // UserName ile giriş yap (Identity UserName ile çalışır)
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Claims ekle
                var claims = new List<Claim>
                {
                    new Claim("FullName", $"{user.FirstName} {user.LastName}")
                };

                // Mevcut sign-in'i iptal et ve claims ile yeniden sign in yap
                await _signInManager.SignOutAsync();
                await _signInManager.SignInWithClaimsAsync(
                    user,
                    model.RememberMe,
                    claims);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyin.");
                return View(model);
            }

            ModelState.AddModelError("", "Email veya şifre hatalı");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        public async Task<IActionResult> AccountProfile()
        {
            var user = await _userManager.Users
                .Include(u => u.Faculty)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(_userManager.GetUserId(User)));
            return View("AccountProfile", user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.Users
                .Include(u => u.Faculty)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(_userManager.GetUserId(User)));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var faculties = await _facultyService.GetAllFacultiesAsync();
            var departments = user.FacultyId.HasValue
                ? await _departmentService.GetDepartmentsByFacultyIdAsync(user.FacultyId.Value)
                : new List<Data.DTOs.DepartmentDto.DepartmentListDto>();

            var model = new UserProfileEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                StudentNumber = user.StudentNumber,
                PhoneNumber = user.PhoneNumber,
                FacultyId = user.FacultyId,
                DepartmentId = user.DepartmentId,
                Faculties = faculties,
                Departments = departments
            };

            // Öğrenci ise numarayı email'den türetip göster (Veritabanında kayıtlı olmasa bile anlık gösterim)
            if (User.IsInRole("Student") && !string.IsNullOrEmpty(user.Email) && user.Email.Contains("@"))
            {
                model.StudentNumber = user.Email.Split('@')[0];
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                model.Departments = model.FacultyId.HasValue
                    ? await _departmentService.GetDepartmentsByFacultyIdAsync(model.FacultyId.Value)
                    : new List<Data.DTOs.DepartmentDto.DepartmentListDto>();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("AccountProfile");
            }

            try
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;
                // Öğrenci numarasını email adresinden türet (backend validation) - Sadece Öğrenciler İçin
                if (User.IsInRole("Student"))
                {
                    if (!string.IsNullOrEmpty(model.Email) && model.Email.Contains("@"))
                    {
                        user.StudentNumber = model.Email.Split('@')[0];
                    }
                }
                else
                {
                    user.StudentNumber = model.StudentNumber;
                }
                user.PhoneNumber = model.PhoneNumber;
                user.FacultyId = model.FacultyId;
                user.DepartmentId = model.DepartmentId;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                    return RedirectToAction("AccountProfile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model.Faculties = await _facultyService.GetAllFacultiesAsync();
            model.Departments = new List<Data.DTOs.DepartmentDto.DepartmentListDto>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentsByFaculty(Guid facultyId)
        {
            var departments = await _departmentService.GetDepartmentsByFacultyIdAsync(facultyId);
            var result = departments
                .Select(d => new { id = d.Id, name = d.DepartmentName })
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

