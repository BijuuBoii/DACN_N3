﻿using DACN_N3.Data;
using DACN_N3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

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

            var topMovies = _movieDbContext.Movies
     .Include(m => m.Reviews)
     .Select(m => new
     {
         MovieId = m.MovieId,
         Title = m.Title,
         Poster = m.Poster,
         Reviews = m.Reviews  // Include reviews data for client-side calculation
     })
     .AsEnumerable()  // Switch to client-side execution
     .Select(m => new
     {
         m.MovieId,
         m.Title,
         m.Poster,
         AverageRating = m.Reviews
             .Where(r => r.Rating.HasValue)
             .GroupBy(r => r.UserId)
             .Select(g => g.Average(r => r.Rating.Value))
             .DefaultIfEmpty(0)  // Default to 0 if no ratings
             .Average()  // Compute the average of the group averages (or 0 if no ratings)
     })
     .OrderByDescending(m => m.AverageRating)  // Sort by the average rating (or 0 if no reviews)
     .Take(10)  // Take top 10 movies
     .ToList();

            // Chuyển dữ liệu sang ViewModel hoặc ViewBag
            ViewBag.TopMovies = topMovies;

			return View(movies);
        }
		[HttpGet]
		public IActionResult Search(string keyword)
		{
			var movies = _movieDbContext.Movies
				.Where(m => m.Title.Contains(keyword) || // Tìm kiếm theo tiêu đề
				m.Description.Contains(keyword) || // Tìm kiếm theo nội dung
				m.Director.Contains(keyword) || // Tìm kiếm theo đạo diễn
				m.Cast.Contains(keyword) || // Tìm kiếm theo dàn diễn viên
				m.Language.Contains(keyword) || // Tìm kiếm theo ngôn ngữ
				m.Genres.Any(g => g.Name.Contains(keyword))) // Tìm kiếm theo thể loại
				.ToList();
            ViewBag.RelatedFilms = movies;
			return View("ListFilm");
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
			if (!User.Identity.IsAuthenticated)
			{
				TempData["buyAlert"] = "Vui lòng đăng nhập để đặt vé xem phim";
				return RedirectToAction("Index", "Home");
			}
			return View();
        }

        public IActionResult movieDetails()
        {
			

			return View();
        }

        public IActionResult SelectChair(string date, string time,string cinema)
        {
			List<CinemaTicket> seats = new List<CinemaTicket>();
			DateTime selectedDateTime;
			string combinedDateTime = date + " " + time;
			if (DateTime.TryParseExact(combinedDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDateTime))
			{
				seats = _movieDbContext.CinemaTickets
					.Where(s => s.ShowTime == selectedDateTime)
					.ToList();
			}

			// Lấy danh sách các số ghế đã đặt và xóa khoảng trống
			var bookedSeats = seats.Select(s => s.SeatNumber.Trim()).ToList();


			HttpContext.Session.SetString("SelectedDate", date);
			HttpContext.Session.SetString("SelectedTime", time);
			HttpContext.Session.SetString("SelectedCinema", cinema);

			ViewData["SelectedDate"] = date;
			ViewData["SelectedTime"] = time;
			ViewData["SelectedCinema"] = cinema;
			ViewData["BookedSeats"] = bookedSeats; // Truyền danh sách ghế đã đặt

			return View(seats); // Trả về view chọn ghế

		}

		[HttpPost]
		public IActionResult SaveSelectedSeatsToSession(string selectedSeats)
		{
			HttpContext.Session.SetString("SelectedSeats", selectedSeats);
			return Ok();
		}


		public IActionResult selectTime()
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

			var hasEpisodes = movie.Seasons.Any(s => s.Episodes.Any());
			if (!hasEpisodes)
            {
				TempData["NoEpAlert"] = "phim này hiện tại chưa có tập nào, vui lòng quay lại sau!";
				return RedirectToAction("Index", "Home"); // Giả sử trang đăng ký gói là "Subscription"
			}
            var comments = _movieDbContext.Reviews.Where(s=>s.MovieId == id).Include(s=>s.User).ToList();

			var movieFav = _movieDbContext.Watchlists.Where(m => m.MovieId == id && m.UserId == userId).FirstOrDefault();

			var selectedGenres = movie?.Genres.Select(g => g.GenreId).ToList();

			var similarMovies = _movieDbContext.Movies
	        .Where(m => m.Genres.Any(g => selectedGenres.Contains(g.GenreId)) && m.MovieId != id)
	        .Include(m => m.Genres) // Bao gồm thể loại của các phim
	        .ToList();

			ViewBag.MovieFav = movieFav;
			ViewBag.Comments = comments;
			ViewBag.SeasonNumber = 1;
			ViewBag.Movie = movie;
			ViewBag.Seasons = movie.Seasons.ToList();
			ViewBag.Episodes = movie.Seasons.SelectMany(s => s.Episodes).ToList();
            ViewBag.Genres = movie.Genres.ToList();
            ViewBag.similarMovies = similarMovies;
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
		[HttpPost]
		public async Task<IActionResult> Favorite(int movieId)
        {
			int? userId = HttpContext.Session.GetInt32("userID");
			var movieFav = _movieDbContext.Watchlists.Where(m=>m.MovieId == movieId && m.UserId == userId).FirstOrDefault();
            if(movieFav == null)
            {
				
				Watchlist watchlist = new Watchlist
                {
                    MovieId = movieId,
                    UserId = userId,
                    AddedDate = DateTime.Now
                };
				_movieDbContext.Watchlists.Add(watchlist);
				await _movieDbContext.SaveChangesAsync();
			}
            else
            {
				_movieDbContext.Watchlists.Remove(movieFav);
				await _movieDbContext.SaveChangesAsync();
			}

			return RedirectToAction("FilmDetails", new { id = movieId });
		}
		public IActionResult ListFilmFav()
        {
			int? userId = HttpContext.Session.GetInt32("userID");
			var watchlist = _movieDbContext.Watchlists
								.Where(w => w.UserId == userId)
								.Include(w => w.Movie) // Bao gồm thông tin phim
								.ToList();
            
			ViewBag.RelatedFilms = watchlist;
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
