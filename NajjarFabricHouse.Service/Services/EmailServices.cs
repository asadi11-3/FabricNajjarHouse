using MimeKit;
using MailKit.Net.Smtp;
using System;
using NajjarFabricHouse.Service.Models;

namespace NajjarFabricHouse.Service.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailServices(EmailConfiguration emailConfiguration) =>
            _emailConfiguration = emailConfiguration;

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Support", _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(message.IsHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
            {
                Text = message.Content
            };
            return emailMessage;
        }

        private void Send(MimeMessage mimeMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
            finally
            {
                client.Disconnect(true);
            }
        }
    }
}
