using Microsoft.AspNetCore.Mvc;

namespace DACN_N3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult movies()
		{
			return View();
		}

		public IActionResult genres()
		{
			return View();
		}

		public IActionResult users()
		{
			return View();
		}

		public IActionResult settings()
		{
			return View();
		}

        public IActionResult addMovies()
        {
            return View();
        }
    }
}
