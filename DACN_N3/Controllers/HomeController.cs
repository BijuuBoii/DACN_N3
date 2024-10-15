using DACN_N3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DACN_N3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
       
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult IndexXem()
        {
            return RedirectToAction("FilmDetails", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult FilmDetails()
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
