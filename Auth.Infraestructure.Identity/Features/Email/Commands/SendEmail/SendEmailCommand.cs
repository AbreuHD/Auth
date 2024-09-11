using Auth.Core.Application.Settings;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using MimeKit;

namespace Auth.Infraestructure.Identity.Features.Email.Commands.SendEmail
{
    public class SendEmailCommand : IRequest<bool>
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, bool>
    {
        private MailSettings _mailSettings { get; }
        public SendEmailCommandHandler(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                MimeMessage email = new();
                email.Sender = MailboxAddress.Parse($"{_mailSettings.DisplayName} <{_mailSettings.EmailFrom}>");
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailFrom));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                BodyBuilder bodyBuilder = new();
                bodyBuilder.HtmlBody = request.Body;
                email.Body = bodyBuilder.ToMessageBody();

                using SmtpClient smtp = new();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.SslOnConnect, cancellationToken);
                smtp.Authenticate(_mailSettings.SmtpUser, _mailSettings.SmtpPassword, cancellationToken);
                await smtp.SendAsync(email);
                smtp.Disconnect(true, cancellationToken);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
