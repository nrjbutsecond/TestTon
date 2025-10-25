using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailConfirmationAsync(string email, string fullName, string confirmationToken);
        Task SendPasswordResetAsync(string email, string fullName, string resetToken);
        Task SendTicketAsync(string email, string fullName, string eventName, byte[] qrCode, string ticketCode);
        Task SendBulkEmailAsync(List<EmailMessage> messages);
    }

    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
    }
}
