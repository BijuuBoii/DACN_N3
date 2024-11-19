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
        public int GetNewUsersThisMonth()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var newUsersCount = _movieDbContext.Users
                .Where(u => u.CreatedDate >= startOfMonth && u.CreatedDate <= endOfMonth)
                .Count();

            return newUsersCount;
        }
        
        public IActionResult Index()
        {
            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalRevenueTicket = 0 + " VND";
            
            // Lấy danh sách tất cả các UserSubscription từ cơ sở dữ liệu
            var userSubscriptions = _movieDbContext.UserSubscriptions.Include(us => us.Subscription).ToList();

            // Tính tổng tiền của tất cả các Subscription
            decimal totalAmount = 0;
            foreach (var userSubscription in userSubscriptions)
            {
                if (userSubscription.Subscription != null)
                {
                    totalAmount += userSubscription.Subscription.Price * (userSubscription.Subscription.Duration);
                }
            }
            string formattedTotalRevenue = totalAmount.ToString("N0", new System.Globalization.CultureInfo("vi-VN"));
            ViewBag.TotalRevenueSub = formattedTotalRevenue + " VND"; // Tổng doanh thu
            var UserAmount = _movieDbContext.Users.Count();
            ViewBag.TotalUsers =UserAmount; // Tổng số người dùng

            var MovieAmount = _movieDbContext.Movies.Count();
            ViewBag.TotalMovies = MovieAmount; // Tổng số phim

            var genresWithMovieCount = _movieDbContext.Genres
                                       .Select(g => new
                                       {
                                           GenreName = g.Name,
                                           MovieCount = g.Movies.Count()
                                       })
                                       .ToList();

            // Lưu vào ViewBag để truyền dữ liệu ra View
            ViewBag.GenresWithMovieCount = genresWithMovieCount;


            // Thông tin người dùng
            ViewBag.ActiveUsers = 1; // Số người dùng đang hoạt động
                                     // Lấy số người dùng mới trong tháng
            var newUsers = GetNewUsersThisMonth();
            ViewBag.NewUsers = newUsers;

			// Lấy doanh thu từ subscription trong năm nay
			var currentYear = DateTime.Now.Year;

			var revenueData = _movieDbContext.UserSubscriptions
									  .Where(us => us.StartDate.HasValue && us.StartDate.Value.Year == currentYear)
									  .GroupBy(us => us.StartDate.Value.Month) // Nhóm theo tháng
									  .Select(g => new
									  {
										  Month = g.Key,
										  Revenue = g.Sum(us => us.Subscription.Price) // Tổng doanh thu cho từng tháng
									  })
									  .OrderBy(x => x.Month)
									  .ToList();

			// Lưu vào ViewBag
			ViewBag.RevenueData = revenueData;
            decimal totalTicketRevenue = _movieDbContext.CinemaTickets
                                .Sum(ct => ct.TicketPrice); // Cộng tất cả giá trị trong cột Price
            ViewBag.TotalTicketRevenue = totalTicketRevenue.ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + " VND";

            // Tổng số vé đã bán
            var totalTicketsSold = _movieDbContext.CinemaTickets.Count();
            ViewBag.TotalTicketsSold = totalTicketsSold;
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
                if(listURL.Phan != null)
                {
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
					// Tìm số tập bị thiếu
					var existingEpisodes = season.Episodes.Select(e => e.EpisodeNumber).OrderBy(e => e).ToList();
                    int missingEpisodeNumber;
					if (!existingEpisodes.Any())
					{
						missingEpisodeNumber = 1; // Tập đầu tiên
					}
					else
					{
						missingEpisodeNumber = Enumerable.Range(1, existingEpisodes.Max())
							.Except(existingEpisodes)
							.FirstOrDefault();

						if (missingEpisodeNumber == 0)
						{
							missingEpisodeNumber = existingEpisodes.Max() + 1;
						}
					}
					if (missingEpisodeNumber == 0)
					{
						// Nếu không có tập nào bị thiếu, gán giá trị mặc định (ví dụ: tập tiếp theo)
						missingEpisodeNumber = existingEpisodes.Max() + 1;
					}

					// Nếu có tập bị thiếu, thêm tập đó
					if (missingEpisodeNumber != 0)
					{
						var newEpisode = new Episode
						{
							EpisodeNumber = missingEpisodeNumber,
							VideoUrl = "",
							Title = "Tập " + missingEpisodeNumber
						};
						season.Episodes.Add(newEpisode);
					}
					else
					{
						// Nếu không có tập bị thiếu, thêm tập kế tiếp
						var newEpisode = new Episode
						{
							EpisodeNumber = existingEpisodes.Max() + 1,
							VideoUrl = "",
							Title = "Tập " + (existingEpisodes.Max() + 1)
						};
						season.Episodes.Add(newEpisode);
					}

					// Sắp xếp lại danh sách tập theo số thứ tự
					season.Episodes = season.Episodes.OrderBy(e => e.EpisodeNumber).ToList();
					// Lưu thay đổi
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
