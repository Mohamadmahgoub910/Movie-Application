namespace MovieApp.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink);
        Task SendPasswordResetAsync(string toEmail, string userName, string resetLink);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
    }
}