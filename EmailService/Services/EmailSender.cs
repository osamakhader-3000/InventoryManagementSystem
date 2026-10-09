using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SecurityApplicationLogic.Interfaces;
using System.Runtime;

namespace EmailService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }



        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.Name, _settings.Address));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new SmtpClient();


            await smtp.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendVerificationEmail(string email, string verificationLink)
        {
            string subject = "Verify your email address";
            string htmlBody = $"<p>Please verify your email address by clicking the link below:</p><p><a href='{verificationLink}'>Verify Email</a></p>";
            await SendEmailAsync(email, subject, htmlBody);
        }

    }
}
