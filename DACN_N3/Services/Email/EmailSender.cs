using System.Net.Mail;
using System.Net;

namespace DACN_N3.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("comfymovie22@gmail.com", "hzcokhslgliilqqu")
            };

            return client.SendMailAsync(
                new MailMessage(from: "comfymovie22@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
        
    }
}
