using Microsoft.AspNetCore.Mvc;

namespace DACN_N3.Controllers
{
    public class AuthorityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
