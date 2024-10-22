using AppointmentScheduling.Models;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AppointmentScheduling.Services
{
    //public class EmailService
    //{
    //    public async void SendEmail(EmailModel emailModel)
    //    {
    //        var message = new MimeMessage();
    //        message.From.Add(new MailboxAddress("Haris AppointmentScheduler", emailModel.From));
    //        message.To.Add(new MailboxAddress("", emailModel.To));
    //        message.Subject = emailModel.Subject;

    //        message.Body = new TextPart("plain")
    //        {
    //            Text = emailModel.Body
    //        };

    //        using (var client = new SmtpClient())
    //        {
    //            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
    //            client.Authenticate("abbasiharis1997@gmail.com", "lrndjowzdjomqeql");

    //            client.Send(message);
    //            client.Disconnect(true);
    //        }
    //    }
    //}

    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Haris AppointmentScheduler", emailModel.From));  // Updated user name
            message.To.Add(new MailboxAddress("", emailModel.To));
            message.Subject = emailModel.Subject;

            message.Body = new TextPart("plain")
            {
                Text = emailModel.Body
            };

            using (var client = new SmtpClient())
            {
                // Updated client connect configuration for Gmail with TLS
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Updated email and password
                await client.AuthenticateAsync("abbasiharis1997@gmail.com", "YOUR_APP_PASSWORD");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}

