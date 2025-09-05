using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.EmailService
{
    public class Message
    {
        public MailboxAddress To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Content { get; set; } = null!;
        public Message(string name,string address,string subject,string content)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("收件人邮箱地址不能为空", nameof(address));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("邮件主题不能为空", nameof(subject));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("邮件内容不能为空", nameof(content));

            To = new MailboxAddress(name ?? "系统邮件", address);
            Subject = subject;
            Content = content;
        }
    }
}
