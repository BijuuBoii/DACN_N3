using Microsoft.AspNetCore.Mvc;
using DACN_N3.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Hosting;
using DACN_N3.Areas.Admin.Models.editMovie;

namespace DACN_N3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
		private MovieDbContext _movieDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController( MovieDbContext movieDbContext, IWebHostEnvironment webHostEnvironment)
		{
			_movieDbContext = movieDbContext;
            _webHostEnvironment = webHostEnvironment;

        }
		public IActionResult Index()
        {
            return View();
        }

		public IActionResult movies()
		{
            var movies = _movieDbContext.Movies
                       .Include(m => m.Seasons) // Bao gồm các mùa
                       .ThenInclude(s => s.Episodes) // Bao gồm các tập
                       .Include(g => g.Genres)
                       .ToList();

            ViewBag.Movies = movies;
			return View();
		}
        [HttpPost]
        public IActionResult EditMovie(Movie movie, string action, IFormFile HPoster, IFormFile VPoster, MovieViewModel listURL)
        {
            var selectedMovie = _movieDbContext.Movies.Include(m => m.Seasons).ThenInclude(m => m.Episodes).FirstOrDefault(m => m.MovieId == movie.MovieId);
            if (action == "save")
            {
                // lấy phim cần sửa
                
               selectedMovie.Title = movie.Title;
               selectedMovie.Description = movie.Description;
               selectedMovie.Duration = movie.Duration;
                foreach (var phanEntry in listURL.Phan)
                {
                    foreach (var tapEntry in phanEntry.Value.Tap)
                    {
                        // Tìm Season tương ứng trong Movie
                        var season = selectedMovie.Seasons.FirstOrDefault(s => s.SeasonNumber == phanEntry.Key);
                        if (season != null)
                        {
                            // Tìm Episode tương ứng trong Season
                            var episode = season.Episodes.FirstOrDefault(e => e.EpisodeNumber == tapEntry.Key);
                            if (episode != null)
                            {
                                // Cập nhật VideoUrl cho Episode
                                episode.VideoUrl = tapEntry.Value.VideoUrl;
                            }
                        }
                    }
                }
                _movieDbContext.SaveChanges();

            }
            else if (action.Contains("addSeason"))
            {
                // Tạo season mới
                var newSeason = new Season
                {
                    SeasonNumber = (selectedMovie.Seasons.Max(s => (int?)s.SeasonNumber) ?? 0) + 1,
                    Episodes = new List<Episode>()
                };
                selectedMovie.Seasons.Add(newSeason);
                _movieDbContext.SaveChanges();

            }
            else if (action.Contains("addEp"))
            {
                // Tách action thành một mảng các phần tử
                var parts = action.Split(' ');
                // Lấy số mùa từ phần tử đầu tiên
                int seasonNumber = int.Parse(parts[1]);
                // Tìm season cần thêm tập
                var season = selectedMovie.Seasons.FirstOrDefault(s => s.SeasonNumber == seasonNumber);
                if (season != null)
                {
                    var newEpisode = new Episode
                    {
                        EpisodeNumber = (season.Episodes.Max(e => (int?)e.EpisodeNumber) ?? 0) + 1,
                        VideoUrl = "",
                        Title = "Tập " + (season.Episodes.Max(e => (int?)e.EpisodeNumber) ?? 0) + 1

                    };
                    season.Episodes.Add(newEpisode);
                    _movieDbContext.SaveChanges();
                }
            }
            else if (action.StartsWith("deleteEp"))
            {
                // Tách action thành một mảng các phần tử
                var parts = action.Split(' ');

                // Lấy số mùa từ phần tử đầu tiên
                int seasonNumber = int.Parse(parts[1]);

                // Lấy số tập từ phần tử thứ hai
                int episodeNumber = int.Parse(parts[2]);

                // Tìm season cần xóa tập
                var season = selectedMovie.Seasons.FirstOrDefault(s => s.SeasonNumber == seasonNumber);
                if (season != null)
                {
                    var episodeToDelete = season.Episodes.FirstOrDefault(e => e.EpisodeNumber == episodeNumber);
                    if (episodeToDelete != null)
                    {
                        season.Episodes.Remove(episodeToDelete); // Xóa tập
                        _movieDbContext.SaveChanges();
                    }
                }
            }
            else if (action.StartsWith("deleteSeason"))
            {
                // Lấy số từ cuối của action (ví dụ: "deleteSeason 2")
                int seasonNumber = int.Parse(action.Split(' ').Last()); // Lấy số cuối của action (season number)

                // Tìm và xóa season
                var season = selectedMovie.Seasons.FirstOrDefault(s => s.SeasonNumber == seasonNumber);
                if (season != null)
                {
                    selectedMovie.Seasons.Remove(season); // Xóa mùa
                    _movieDbContext.SaveChanges();
                }
            }

            else if (action == "delete")
            {
                var deleteMovie = _movieDbContext.Movies.Where(m => m.MovieId == movie.MovieId).Include(m=>m.Genres).FirstOrDefault();
                var relatedGenres = deleteMovie.Genres.Where(m=>m.Movies.Any(g => g.MovieId == movie.MovieId)).ToList();
                var favorite = _movieDbContext.Watchlists.Where(w => w.MovieId == movie.MovieId).ToList();
                var comment = _movieDbContext.Reviews.Where(w => w.MovieId == movie.MovieId).ToList();
                foreach(var c in comment)
                {
                    _movieDbContext.Reviews.Remove(c);
                }
                foreach (var f in favorite)
                {
                    _movieDbContext.Watchlists.Remove(f);
                }
                foreach (var g in relatedGenres)
                {
                    deleteMovie.Genres.Remove(g);
                }
                var HimagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", deleteMovie.HorizontalPoster);
                var VimagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", deleteMovie.Poster);
                // Xóa tệp hình ảnh
                System.IO.File.Delete(HimagePath);
                System.IO.File.Delete(VimagePath);

                _movieDbContext.Movies.Remove(deleteMovie);
                _movieDbContext.SaveChanges();
            }
            

            // Sau khi xử lý, có thể chuyển hướng về một trang khác hoặc trả về kết quả
            return RedirectToAction("movies");
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
            // Lấy danh sách người dùng không phải Admin và gói đăng ký của họ
            var usersWithSubscriptions = _movieDbContext.Users
                .Where(u => u.Role != "Admin") // Loại bỏ Admin
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    Subscriptions = u.UserSubscriptions
                        .Where(us => us.EndDate > DateTime.Now) // Chỉ gói còn hiệu lực
                        .Select(us => us.Subscription.Name)
                        .ToList()
                })
                .ToList();
            return View(usersWithSubscriptions);
		}

		public IActionResult settings()
		{
			return View();
		}

        public IActionResult addMovies()
        {
            var genres = _movieDbContext.Genres.ToList();
            ViewBag.genres = genres;
            return View();
        }
		[HttpPost]
		public async Task<IActionResult> addMovie(IFormFile VImage, IFormFile HImage, Movie movie, string genres)
		{
			if (ModelState.IsValid)
			{
				string sanitizedTitle = movie.Title.Replace(" ", "");
				// Kiểm tra và lưu hình ảnh
				if (VImage != null)
				{
					var extension = Path.GetExtension(VImage.FileName).ToLower();
					var vImageFileName = $"{sanitizedTitle}{extension}"; // Kết hợp tiêu đề với định dạng gốc
					var vImagePath = Path.Combine("wwwroot/img/", vImageFileName);

					using (var stream = new FileStream(vImagePath, FileMode.Create))
					{
						await VImage.CopyToAsync(stream);
					}

					movie.Poster = vImageFileName; // Lưu đường dẫn hình ảnh vào cơ sở dữ liệu
				}

				if (HImage != null)
				{
					var hImageExtension = Path.GetExtension(HImage.FileName).ToLower(); // Lấy định dạng gốc
					var hImageFileName = $"{sanitizedTitle}H{hImageExtension}"; // Thêm 'H' để phân biệt hình ngang
					var hImagePath = Path.Combine("wwwroot/img/", hImageFileName);

					using (var stream = new FileStream(hImagePath, FileMode.Create))
					{
						await HImage.CopyToAsync(stream);
					}

					movie.HorizontalPoster = hImageFileName; // Lưu đường dẫn hình ảnh vào cơ sở dữ liệu
				}

				// Lưu phim vào cơ sở dữ liệu
				_movieDbContext.Movies.Add(movie);
				await _movieDbContext.SaveChangesAsync();

                // Lưu thể loại
                // Phân tách danh sách genres đã chọn
                var selectedGenres = genres.Split(',').ToList();

                // Lưu các thể loại vào cơ sở dữ liệu
                foreach (var genreName in selectedGenres)
                {
                    var genre = await _movieDbContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                    if (genre != null)
                    {
                        movie.Genres.Add(genre);
                    }
                }
                await _movieDbContext.SaveChangesAsync();

				return RedirectToAction("movies"); // Hoặc hành động khác
			}
			return View("addMovies");
		}
	}
}
