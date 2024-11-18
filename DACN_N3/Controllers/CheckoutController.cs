using DACN_N3.Data;
using DACN_N3.Models;
using DACN_N3.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DACN_N3.Data;
using DACN_N3.Services.Email;
using System.Globalization;
namespace DACN_N3.Controllers
{
	public class CheckoutController : Controller
	{
		
		private IMomoService _momoService;
		private MovieDbContext _movieDbContext;
		private readonly IEmailSender _emailSender;
		public CheckoutController(IMomoService momoService, MovieDbContext movieDbContext, IEmailSender emailSender)
		{
			_momoService = momoService;
			_movieDbContext = movieDbContext;
			_emailSender = emailSender;
		}
		//phương thức gọi khi chạy bất kì action nào
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);
			var genres = _movieDbContext.Genres.ToList();
			ViewBag.AllGenres = genres;

		}
		public IActionResult Index()
		{
			return View();
		}
		private string[] seatArray;
		[HttpPost]
		public IActionResult SaveSeats(string SelectedSeats) {
			if (!string.IsNullOrEmpty(SelectedSeats))
			{
				// Tách chuỗi thành mảng
				seatArray = SelectedSeats.Split(',');
			}
			else
			{
				ViewBag.Message = "Vui lòng nhập ghế!";
			}
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> PaymentCallBack()
		{
			var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
			var requestQuery = HttpContext.Request.Query;
			if (requestQuery["errorCode"] == "0")
			{
				int? userId = HttpContext.Session.GetInt32("userID");
				
				int? subscriptionId = _movieDbContext.Subscriptions.Where(p => p.Price == decimal.Parse(requestQuery["Amount"])).Select(s=>s.SubscriptionId).FirstOrDefault();
				int duration = _movieDbContext.Subscriptions.Where(p => p.Price == decimal.Parse(requestQuery["Amount"])).Select(s => s.Duration).FirstOrDefault();
				DateTime startDate = DateTime.Now;

				
				if (requestQuery["extraData"] == "DkGoi")
				{


					UserSubscription userSubscription = new UserSubscription
					{
						UserId = userId,
						SubscriptionId = subscriptionId,
						StartDate = startDate,
						EndDate = startDate.AddDays(duration)
					};
					_movieDbContext.Add(userSubscription);

					var receiver = "Datcopw123@gmail.com";
					var subject = "Thanh toán gói tháng ComfyMovie";
					var message = "Thanh toán thành công gói tháng, chúc bạn có những phút giây xem phim thư giản";
					await _emailSender.SendEmailAsync(receiver, subject, message);
				}
				string selectedDate = HttpContext.Session.GetString("SelectedDate");
				string selectedTime = HttpContext.Session.GetString("SelectedTime");
				string selectedCinema = HttpContext.Session.GetString("SelectedCinema");
				string selectedSeat = HttpContext.Session.GetString("SelectedSeat");
				DateTime selectedDateTime;
			if (!string.IsNullOrEmpty(selectedDate) && !string.IsNullOrEmpty(selectedTime))
				{
					string combinedDateTime = selectedDate + " " + selectedTime;
					if (DateTime.TryParseExact(combinedDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDateTime))
					{
						// Tiến hành lưu thông tin vé vào cơ sở dữ liệu
						if (requestQuery["extraData"] == "MuaVe")
						{
							
							// Lấy số ghế từ request hoặc từ thông tin khác
							decimal ticketPrice = decimal.Parse(requestQuery["Amount"]);
							DateTime bookingDate = DateTime.Now;
							string selectedSeats = selectedSeat;
							string[] seatNumbers = selectedSeats.Split(',');
							foreach (var seatNumber in seatNumbers)
							{
								CinemaTicket cinemaTicket = new CinemaTicket
								{
									UserId = userId,
									SeatNumber = seatNumber,
									TicketPrice = decimal.Parse(requestQuery["Amount"]),
									BookingDate = DateTime.Now,
									// Lưu các thông tin khác vào database nếu cần
								};
								_movieDbContext.Add(cinemaTicket);
							}
							await _movieDbContext.SaveChangesAsync();

							// Thông báo thành công hoặc thực hiện các hành động khác
							TempData["SuccessMessage"] = "Thanh toán và đặt vé thành công!";
							var receiver = "Datcopw123@gmail.com";
							var subject = "Thanh toán gói tháng ComfyMovie";
							var message = "Thanh toán đặt vé thành công, chúc bạn có những phút giây xem phim thư giản";
							await _emailSender.SendEmailAsync(receiver, subject, message);
						}
					}
					else
					{
						ViewData["ErrorMessage"] = "Ngày và giờ không hợp lệ.";
					}
				}
				await _movieDbContext.SaveChangesAsync();
            }
			else
			{
				TempData["success"] = "Đã hủy giao dịch Momo.";
				return RedirectToAction("subscription", "Home");
			}
			return View(response);
		}
	}
}
