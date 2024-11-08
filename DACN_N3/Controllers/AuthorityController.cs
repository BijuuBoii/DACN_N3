using DACN_N3.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
		public async Task<IActionResult> Login(User user)
		{
			

			var genres = _movieDbContext.Genres.ToList(); // Lấy danh sách thể loại
			ViewBag.AllGenres = genres; // Gửi danh sách thể loại vào ViewBag

			var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == user.Email);
			if (existingUser != null && user.Password == existingUser.Password)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, existingUser.Username),
					new Claim(ClaimTypes.Email, existingUser.Email),
					new Claim(ClaimTypes.Role, existingUser.Role)
				};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var authProperties = new AuthenticationProperties
				{
					IsPersistent = true // Cookie tồn tại sau khi đóng trình duyệt
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
				// Đăng nhập thành công
				if(existingUser.Role == "User")
					return RedirectToAction("Index", "Home");
				else if (existingUser.Role == "Admin")
					return RedirectToAction("Home", "Admin");
			}
			// Xử lý đăng nhập không thành công
			ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ.");
			return View(user);
		}
		[HttpPost]
		public IActionResult SignUp(User user)
		{
			var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == user.Email);

			if (user.Password != null && user.Email != null && user.Username != null && existingUser == null)
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
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}


	}
}
