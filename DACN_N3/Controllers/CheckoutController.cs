using DACN_N3.Data;
using DACN_N3.Models;
using DACN_N3.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DACN_N3.Data;
namespace DACN_N3.Controllers
{
	public class CheckoutController : Controller
	{
		private IMomoService _momoService;
		private MovieDbContext _movieDbContext;
		public CheckoutController(IMomoService momoService, MovieDbContext movieDbContext)
		{
			_momoService = momoService;
			_movieDbContext = movieDbContext;
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
                    
                }
				if (requestQuery["extraData"] == "")
				{

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
