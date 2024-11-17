using DACN_N3.Models;
using DACN_N3.Services.Email;
using DACN_N3.Services.Momo;
using Microsoft.AspNetCore.Mvc;

namespace DACN_N3.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        private readonly IEmailSender _emailSender;
        public PaymentController(IEmailSender emailSender,IMomoService momoService)
        {
            _momoService = momoService;
            _emailSender = emailSender;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentMomo(model);
			return Redirect(response.PayUrl);

        }
		[HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
			var receiver = "Datcopw123@gmail.com";
			var subject = "Thanh toán gói tháng ComfyMovie";
			var message = "Thanh toán đặt vé thành công, chúc bạn có những phút giây xem phim thư giản";
			await _emailSender.SendEmailAsync(receiver, subject, message);
			return View(response);
        }
    }
}
