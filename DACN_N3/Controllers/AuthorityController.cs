﻿using DACN_N3.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DACN_N3.Services.Email;

namespace DACN_N3.Controllers
{
    public class AuthorityController : Controller
    {
		private MovieDbContext _movieDbContext;
		private readonly IEmailSender _emailSender;
        public AuthorityController( MovieDbContext movieDbContext, IEmailSender emailSender)
		{
			_movieDbContext = movieDbContext;
			_emailSender = emailSender;
        }
		public IActionResult Index()
        {
			
            return View();

        }
		public async Task<IActionResult> Login()
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
					
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

				// Lưu `userID` vào Session
				HttpContext.Session.SetInt32("userID", existingUser.UserId);

				// Đăng nhập thành công
				if (existingUser.Role == "User")
					return RedirectToAction("Index", "Home");
				else if (existingUser.Role == "Admin")
					return RedirectToAction("Home", "Admin");
			}
			// Xử lý đăng nhập không thành công
			
			return View(user);
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(User user)
		{
			var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == user.Email);

			if (user.Password != null && user.Email != null && user.Username != null && existingUser == null)
			{
				user.Password = user.Password;
				user.CreatedDate = DateTime.Now;
				user.Role = "User";
				_movieDbContext.Users.Add(user);
				_movieDbContext.SaveChanges();
				TempData["SignUpSuccessAlert"] = "Đăng ký thành công";
			}
			else
			{
				TempData["SignUpFailAlert"] = "Đăng ký không thành công";
			}
			return RedirectToAction("Login");
			
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}


	}
}
