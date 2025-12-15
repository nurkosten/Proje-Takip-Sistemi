using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.MVCUI.Models;
using System.Diagnostics;
using System.Security.AccessControl;

namespace ProjeHavuzu.MVCUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {          

                   

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
