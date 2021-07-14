using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Tms2.Services
{
    public class AuthMessageSenderOptions
    { 
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }       

        public string SendGridFromEmail { get; set; }
    }

    public class EmailSender : IEmailSender
    {      
        public EmailSender(IConfiguration configuration)
        {
            Options = new AuthMessageSenderOptions
            {
                SendGridKey = configuration.GetValue("Email_api_key", ""),
                SendGridUser = configuration.GetValue("Email_from_user", ""),
                SendGridFromEmail = configuration.GetValue("Email_from_email", "")                
            };
        }

        public AuthMessageSenderOptions Options { get; set; } 

        public Task SendEmailAsync2(string email, string subject, string message)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var from = new EmailAddress(Options.SendGridFromEmail, Options.SendGridUser);
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            return client.SendEmailAsync(msg);

            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("Joe@contoso.com", Options.SendGridUser),
            //    Subject = subject,
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};
            //msg.AddTo(new EmailAddress(email));          
        }

        public Task SendEmailAsync(string email, string subject, string messageText)
        {
            return Task.Run(() =>
            {
                MailAddress from = new(Options.SendGridFromEmail, Options.SendGridUser);
                MailAddress to = new(email);
                MailMessage message = new(from, to)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = messageText
                };

                SmtpClient smtp = new("smtp.mail.ru", 25)
                {
                    Credentials = new NetworkCredential(Options.SendGridFromEmail, Options.SendGridKey),
                    EnableSsl = true
                };
                smtp.Send(message);
            });
        }
    }
}
