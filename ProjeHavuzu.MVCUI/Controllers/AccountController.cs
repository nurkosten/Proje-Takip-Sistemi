using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.MVCUI.Models.AccountModels;
using ProjeHavuzu.MVCUI.Models.LoginModels;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ProjeHavuzu.MVCUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false);
            var user = await _userManager.FindByEmailAsync(model.Email);


            if (result.Succeeded)
            {
                var claims = new List<Claim>
                        {
                          new Claim("FullName", $"{user.FirstName} {user.LastName}")
                        };

                await _signInManager.SignInWithClaimsAsync(
                    user,
                    model.RememberMe,
                    claims
                );

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Email veya şifre hatalı");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
