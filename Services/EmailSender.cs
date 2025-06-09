using Microsoft.AspNetCore.Identity.UI.Services;

namespace Chill_Closet.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Logic gửi email thật sẽ được thêm vào đây sau (dùng SMTP, SendGrid,...)
            // Tạm thời, chúng ta chỉ cần in ra Console để debug.
            Console.WriteLine("--- NEW EMAIL ---");
            Console.WriteLine($"To: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine(htmlMessage);

            return Task.CompletedTask;
        }
    }
}