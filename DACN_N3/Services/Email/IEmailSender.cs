namespace DACN_N3.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email,string subject,string message);//ham gui mail
    }
}
