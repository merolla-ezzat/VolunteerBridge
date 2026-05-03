using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace VolunteerBridge.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string userName, string confirmLink)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]
            ));
            email.To.Add(new MailboxAddress(userName, toEmail));
            email.Subject = "تأكيد البريد الإلكتروني";
            email.Body = new TextPart("html")
            {
                Text = $@"
                <div dir='rtl' style='font-family: Arial; padding: 20px;'>
                    <h2 style='color: #006e2f;'>مرحباً {userName} 👋</h2>
                    <p>شكراً لتسجيلك في وصال!</p>
                    <p>اضغط على الزرار التالي لتأكيد بريدك الإلكتروني:</p>
                    <a href='{confirmLink}' style='background-color: #22c55e; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; display: inline-block;'>
                        تأكيد الحساب
                    </a>
                    <p style='color: #666; margin-top: 20px;'>لو مش أنت اللي سجلت، تجاهل الإيميل ده.</p>
                </div>"
            };

            try
            {
                using var smtp = new SmtpClient();

                _logger.LogInformation("Connecting to SMTP: {Host}:{Port}",
                    _config["EmailSettings:Host"], _config["EmailSettings:Port"]);

                await smtp.ConnectAsync(
                    _config["EmailSettings:Host"],
                    int.Parse(_config["EmailSettings:Port"]!),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]
                );

                _logger.LogInformation("Sending email to {Email}", toEmail);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw; // Don't swallow — let the caller know it failed
            }
        }
    }
}