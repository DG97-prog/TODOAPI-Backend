using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using TodoApp.API.Interfaces;

namespace TodoApp.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"]);
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Password"];
            var from = _config["Smtp:From"];
            //Console.WriteLine(pass);
            //using var client = new SmtpClient(host, port)
            //{
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential(user, pass)
            //};

            //var mail = new MailMessage(from, to, subject, body)
            //{
            //    IsBodyHtml = false
            //};

            //await client.SendMailAsync(mail);
            var client = new SendGridClient(pass);
            var fromEmail = new EmailAddress(from);
            var subjectEmail = subject;
            var toEmail = new EmailAddress(to);
            var plainTextContent = body;
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subjectEmail, plainTextContent, null);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
