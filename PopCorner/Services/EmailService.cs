using PopCorner.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace PopCorner.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = CreateMessage(to, subject, body);

            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(message);
        }

        public async Task SendOtpAsync(string to, string otp)
        {
            var subject = "Your OTP code";
            var body = $"""
                Your OTP code is: {otp}

                This code will expire in 5 minutes.
                If you did not request this, please ignore this email.
                """;

            await SendAsync(to, subject, body);
        }

        // ===== Helpers =====

        private MailMessage CreateMessage(string to, string subject, string body)
        {
            var from = _config["Smtp:From"];

            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(to);
            return message;
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(
                    _config["Smtp:Username"],
                    _config["Smtp:Password"]
                ),
                EnableSsl = true
            };
        }
    }
}
