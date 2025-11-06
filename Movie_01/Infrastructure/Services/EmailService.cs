using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Settings;

namespace MovieApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error sending email: {ex.Message}");
            }
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink)
        {
            var subject = "تأكيد البريد الإلكتروني - Movie App";
            var body = $@"
                <html dir='rtl'>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
                        .container {{ background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .button {{ display: inline-block; background-color: #667eea; color: white; padding: 15px 30px; 
                                  text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; color: #666; margin-top: 20px; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>مرحباً بك في Movie App</h1>
                        </div>
                        <div class='content'>
                            <h2>مرحباً {userName}،</h2>
                            <p>شكراً لتسجيلك في Movie App!</p>
                            <p>يرجى تأكيد بريدك الإلكتروني بالضغط على الزر التالي:</p>
                            <center>
                                <a href='{confirmationLink}' class='button'>تأكيد البريد الإلكتروني</a>
                            </center>
                            <p style='margin-top: 20px; color: #666;'>إذا لم تقم بإنشاء هذا الحساب، يرجى تجاهل هذه الرسالة.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Movie App. جميع الحقوق محفوظة.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetAsync(string toEmail, string userName, string resetLink)
        {
            var subject = "إعادة تعيين كلمة المرور - Movie App";
            var body = $@"
                <html dir='rtl'>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
                        .container {{ background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .button {{ display: inline-block; background-color: #667eea; color: white; padding: 15px 30px; 
                                  text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; color: #666; margin-top: 20px; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>إعادة تعيين كلمة المرور</h1>
                        </div>
                        <div class='content'>
                            <h2>مرحباً {userName}،</h2>
                            <p>لقد تلقينا طلباً لإعادة تعيين كلمة المرور لحسابك.</p>
                            <p>اضغط على الزر التالي لإعادة تعيين كلمة المرور:</p>
                            <center>
                                <a href='{resetLink}' class='button'>إعادة تعيين كلمة المرور</a>
                            </center>
                            <div class='warning'>
                                <strong>تحذير:</strong> هذا الرابط صالح لمدة ساعة واحدة فقط.
                            </div>
                            <p style='color: #666;'>إذا لم تطلب إعادة تعيين كلمة المرور، يرجى تجاهل هذه الرسالة.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Movie App. جميع الحقوق محفوظة.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "مرحباً بك في Movie App!";
            var body = $@"
                <html dir='rtl'>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
                        .container {{ background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .footer {{ text-align: center; color: #666; margin-top: 20px; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🎬 مرحباً بك في Movie App</h1>
                        </div>
                        <div class='content'>
                            <h2>أهلاً {userName}،</h2>
                            <p>تم تفعيل حسابك بنجاح!</p>
                            <p>يمكنك الآن الاستمتاع بجميع ميزات Movie App:</p>
                            <ul>
                                <li>تصفح آلاف الأفلام</li>
                                <li>متابعة نجومك المفضلين</li>
                                <li>معرفة مواعيد العروض في السينمات</li>
                                <li>والمزيد!</li>
                            </ul>
                            <p>نتمنى لك تجربة ممتعة!</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Movie App. جميع الحقوق محفوظة.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}