using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Xml;


namespace Plaza.Net.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly FromEmailConfig _fromEmail;

        public EmailSender(IOptions<FromEmailConfig> formEmail)
        {
            _fromEmail = formEmail.Value;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);
            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var emailMessage = new MimeMessage();
            Console.WriteLine("发信人信息："+_fromEmail.Address+"下一个"+_fromEmail.Name);
            // 验证发件人配置
            if (string.IsNullOrWhiteSpace(_fromEmail.Address))
                throw new InvalidOperationException("发件人邮箱地址未配置");

            emailMessage.From.Add(new MailboxAddress(_fromEmail.Name, _fromEmail.Address));

            // 确保收件人信息有效
            if (message.To == null)
                throw new ArgumentException("收件人信息无效", nameof(message.To));

            emailMessage.To.Add(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<h2 style='color:red;'>{message.Content}</h2>"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_fromEmail.SmtpServer, _fromEmail.Port, true); // 使用 SSL/TLS
                client.Authenticate(_fromEmail.UserName, _fromEmail.Password);
                client.Send(mailMessage);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_fromEmail.SmtpServer, _fromEmail.Port, true); // 使用 SSL/TLS
                await client.AuthenticateAsync(_fromEmail.UserName, _fromEmail.Password);
                await client.SendAsync(mailMessage);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
