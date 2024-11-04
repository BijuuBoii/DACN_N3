using DACN_N3.Data;
using DACN_N3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DACN_N3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MovieDbContext _movieDbContext;
        public HomeController(ILogger<HomeController> logger, MovieDbContext movieDbContext)
        {
            _logger = logger;
            _movieDbContext = movieDbContext;
        }
       
        public IActionResult Index()
        {
            var movies  = _movieDbContext.Movies.ToList();
            return View(movies);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult subscription()
        {
            return View();
        }

        public IActionResult buyTicket()
        {
            return View();
        }

        public IActionResult movieDetails()
        {
            return View();
        }

        public IActionResult selectChair()
        {
            return View();
        }

        public IActionResult FilmDetails(int id)
        {
			var movie = _movieDbContext.Movies
		    .Include(m => m.Seasons) // Bao gồm các mùa
			.ThenInclude(s => s.Episodes) // Bao gồm các tập
		    .FirstOrDefault(m => m.MovieId == id);

			ViewBag.Movie = movie;
			ViewBag.Seasons = movie.Seasons.ToList();
			ViewBag.Episodes = movie.Seasons.SelectMany(s => s.Episodes).ToList();
			return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
