using DACN_N3.Data;
using DACN_N3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        //phương thức gọi khi chạy bất kì action nào
        public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);
            var genres = _movieDbContext.Genres.ToList();
            ViewBag.AllGenres = genres;
            
		}

        public async Task<IActionResult> Index(int UserID)
        {
            var movies  = _movieDbContext.Movies.Include(m => m.Genres).ToList();
            
            var animeMovies = movies.Where(m => m.Genres.Any(g => g.Name == "Anime")).ToList();
            var crimeMovies = movies.Where(m => m.Genres.Any(g => g.Name == "Crime")).ToList();
            
            var randomMovies = movies.OrderBy(m => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
		    .FirstOrDefault(); // Lấy phim đầu tiên

			ViewBag.AnimeMovies = animeMovies;
            ViewBag.CrimeMovies = crimeMovies;
            ViewBag.RandomMovies = randomMovies;

            
            return View(movies);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult subscription()
        {
			

			var subscriptions = _movieDbContext.Subscriptions.ToList();
            ViewBag.Subscription = subscriptions;

			int? userId = HttpContext.Session.GetInt32("userID");
			var latestSubscription = _movieDbContext.UserSubscriptions
				                          .Where(s => s.UserId == userId)
										  .OrderByDescending(s => s.StartDate)
										  .FirstOrDefault();
            ViewBag.UserSubscription = latestSubscription;
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
            if(!User.Identity.IsAuthenticated)
            {
                TempData["LoginAlert"] = "Vui lòng đăng nhập để xem chi tiết phim!";
                return RedirectToAction("Index", "Home");
            }
            int? userId = HttpContext.Session.GetInt32("userID");
            // Kiểm tra tình trạng đăng ký của người dùng
            var latestSubscription = _movieDbContext.UserSubscriptions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefault();

            if (latestSubscription == null || latestSubscription.EndDate < DateTime.Now)
            {
                // Nếu người dùng chưa đăng ký gói hoặc gói đã hết hạn, yêu cầu đăng ký
                TempData["SubscriptionAlert"] = "Vui lòng đăng ký gói để xem chi tiết phim!";
                return RedirectToAction("Index", "Home"); // Giả sử trang đăng ký gói là "Subscription"
            }

            var movie = _movieDbContext.Movies
		    .Include(m => m.Seasons) // Bao gồm các mùa
			.ThenInclude(s => s.Episodes) // Bao gồm các tập
			.Include(g => g.Genres)
			.FirstOrDefault(m => m.MovieId == id);

            var comments = _movieDbContext.Reviews.Where(s=>s.MovieId == id).Include(s=>s.User).ToList();

            ViewBag.Comments = comments;
			ViewBag.SeasonNumber = 1;
			ViewBag.Movie = movie;
			ViewBag.Seasons = movie.Seasons.ToList();
			ViewBag.Episodes = movie.Seasons.SelectMany(s => s.Episodes).ToList();
            ViewBag.Genres = movie.Genres.ToList();
			return View();
        }
		[HttpPost]
		public IActionResult UpdateFilmDetails(int id, int seasonNumber)
		{
			var movie = _movieDbContext.Movies
			.Include(m => m.Seasons) // Bao gồm các mùa
			.ThenInclude(s => s.Episodes)// Bao gồm các tập
			.Include(g => g.Genres)
			.FirstOrDefault(m => m.MovieId == id);

			ViewBag.SeasonNumber = seasonNumber;
			ViewBag.Movie = movie;
			ViewBag.Seasons = movie.Seasons.ToList();
			ViewBag.Episodes = movie.Seasons.SelectMany(s => s.Episodes).ToList();
			ViewBag.Genres = movie.Genres.ToList();
			return View("FilmDetails"); // Trả về view với dữ liệu mới
		}
		public IActionResult Favorite()
        {
            
            return View();
        }

		public IActionResult ListFilm(int id)
        {
            var movies = _movieDbContext.Movies.Include(g => g.Genres).ToList();
            var relatedFilms = movies.Where(m => m.Genres.Any(g => g.GenreId == id)).ToList();

            var genreName = _movieDbContext.Genres.FirstOrDefault(g => g.GenreId == id)?.Name;
            ViewBag.GenreName = genreName;

            ViewBag.RelatedFilms = relatedFilms;
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Review(string content, int rating, int movieId)
		{
			if (ModelState.IsValid)
			{
				int? userId = HttpContext.Session.GetInt32("userID");
				// Tạo đối tượng bình luận mới
				var Review = new Review
				{
                    UserId = userId,
                    MovieId = movieId,
					Comment = content,
					Rating = rating,
					CreatedDate = DateTime.Now
				};

				// Thêm bình luận vào DbContext và lưu
				_movieDbContext.Reviews.Add(Review);
				await _movieDbContext.SaveChangesAsync();

				// Có thể redirect hoặc trả về kết quả khác sau khi lưu
				
			}
			return RedirectToAction("FilmDetails", new { id = movieId });

		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
