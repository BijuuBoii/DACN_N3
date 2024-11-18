using Microsoft.AspNetCore.Mvc;
using DACN_N3.Data;
using Microsoft.EntityFrameworkCore;

namespace DACN_N3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
		private MovieDbContext _movieDbContext;
		public HomeController( MovieDbContext movieDbContext)
		{
			_movieDbContext = movieDbContext;

		}
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
			var genres = _movieDbContext.Genres.ToList();
			return View(genres);
		}
        // Add Genre
        [HttpPost]
        public IActionResult AddGenre(Genre genre)
        {
            if (ModelState.IsValid)
            {
                _movieDbContext.Genres.Add(genre);
                _movieDbContext.SaveChanges();
                return RedirectToAction("genres");
            }
            return View(genre);
        }

        // Delete Genre
        [HttpPost]
        public IActionResult DeleteGenre(int id)
        {
            var genre = _movieDbContext.Genres
                             .Include(g => g.Movies) // Lấy tất cả các phim liên quan đến thể loại này
                             .FirstOrDefault(g => g.GenreId == id);

            if (genre != null)
            {
                // Xóa thể loại khỏi các phim (nếu có)
                foreach (var movie in genre.Movies.ToList())
                {
                    movie.Genres.Remove(genre); // Xóa liên kết giữa phim và thể loại
                }

                // Bước 2: Xóa thể loại trong bảng Genres
                _movieDbContext.Genres.Remove(genre); // Xóa thể loại khỏi bảng Genres
                _movieDbContext.SaveChanges(); // Lưu thay đổi vào database
            }
            return RedirectToAction("genres");
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
