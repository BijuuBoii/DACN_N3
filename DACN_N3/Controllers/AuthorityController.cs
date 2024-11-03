using DACN_N3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;

namespace DACN_N3.Controllers
{
    public class AuthorityController : Controller
    {
		private MovieDbContext _movieDbContext;
		public AuthorityController(MovieDbContext movieDbContext)
		{
			_movieDbContext = movieDbContext;
		}
		public IActionResult Index()
        {
            return View();
        }
		public IActionResult Login()
		{
			var genres = _movieDbContext.Genres.ToList(); // Lấy danh sách thể loại
			ViewBag.AllGenres = genres; // Gửi danh sách thể loại vào ViewBag
			return View();
		}
		[HttpPost]
		public IActionResult Login(User user)
		{
			var genres = _movieDbContext.Genres.ToList(); // Lấy danh sách thể loại
			ViewBag.AllGenres = genres; // Gửi danh sách thể loại vào ViewBag

			var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == user.Email);
			if (existingUser != null && user.Password == existingUser.Password)
			{
				// Đăng nhập thành công
				return RedirectToAction("Index", "Home");
			}
			// Xử lý đăng nhập không thành công
			ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ.");
			return View(user);
		}
		[HttpPost]
		public IActionResult SignUp(User user)
		{
			if (user.Password != null && user.Email != null && user.Username != null)
			{
				user.Password = user.Password;
				user.CreatedDate = DateTime.Now;
				user.Role = "User";
				_movieDbContext.Users.Add(user);
				_movieDbContext.SaveChanges();
				
			}
			ModelState.AddModelError(string.Empty, "Đăng ký không hợp lệ.");
			return RedirectToAction("Login");

		}



	}
}
