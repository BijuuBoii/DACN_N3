using Microsoft.AspNetCore.Mvc;

namespace DACN_N3.Controllers
{
	public class AdminController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Admin()
		{
			return View();
		}
	}
}
