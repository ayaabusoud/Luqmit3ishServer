using System;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Services
{
    public class EmailSender
    {
        public int code { get; set; } = 0;
        public async Task SendEmailAsync(string recipientName, string recipient)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Luqmit 3ish", "lumit3ish14@gmail.com"));
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = "Welcome to Luqmit 3ish: Verify Your Account";

            var random = new Random();
            code = random.Next(1000, 9999);

            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format("<p>Dear {0},</p>" +
                                             "<p>Your verification code is: <strong>{1}</strong></p>" +
                                             "<p>Thank you for using our service.</p>", recipientName, code);
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync("lumit3ish14@gmail.com", "gdernihsvizqzglq");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }
}
