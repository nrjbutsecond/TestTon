using Application.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MailKit.Net.Smtp;


using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _baseUrl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load SMTP settings from configuration
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@ton.com";
            _fromName = _configuration["EmailSettings:FromName"] ?? "TON Platform";
            _baseUrl = _configuration["EmailSettings:BaseUrl"] ?? "https://localhost:7126";

            // Log config for debugging
            _logger.LogInformation($"Email config - Host: {_smtpHost}, Port: {_smtpPort}, Username: {_smtpUsername}");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var builder = new BodyBuilder();
                if (isHtml)
                    builder.HtmlBody = body;
                else
                    builder.TextBody = body;

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                // Connect - Gmail uses STARTTLS on port 587
                await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);

                // Authenticate
                await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);

                // Send
                await smtp.SendAsync(message);

                // Disconnect
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string email, string fullName, string confirmationToken)
        {
            var confirmUrl = $"{_baseUrl}/api/users/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(confirmationToken)}";

            var subject = "Confirm your email - TON Platform";
            var body = GetEmailConfirmationTemplate(fullName, confirmUrl);

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetAsync(string email, string fullName, string resetToken)
        {
            var resetUrl = $"{_baseUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}";

            var subject = "Reset your password - TON Platform";
            var body = GetPasswordResetTemplate(fullName, resetUrl);

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTicketAsync(string email, string fullName, string eventName, byte[] qrCode, string ticketCode)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = $"Your ticket for {eventName}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetTicketEmailTemplate(fullName, eventName, ticketCode)
                };

                // Attach QR code
                builder.Attachments.Add($"ticket-{ticketCode}.png", qrCode, ContentType.Parse("image/png"));

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Ticket email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send ticket email to {email}");
                throw;
            }
        }

        public async Task SendBulkEmailAsync(List<EmailMessage> messages)
        {
            foreach (var message in messages)
            {
                await SendEmailAsync(message.To, message.Subject, message.Body, message.IsHtml);
                await Task.Delay(100); // Rate limiting
            }
        }

        // Email Templates 
        private string GetEmailConfirmationTemplate(string fullName, string confirmUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f4f4f4; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to TON Platform!</h1>
        </div>
        <div class='content'>
            <p>Hi {fullName},</p>
            <p>Thank you for registering with TON Platform. Please confirm your email address by clicking the button below:</p>
            <p style='text-align: center; margin: 30px 0;'>
                <a href='{confirmUrl}' class='button'>Confirm Email</a>
            </p>
            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all;'>{confirmUrl}</p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't create an account, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 TON Platform. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplate(string fullName, string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f4f4f4; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .button {{ display: inline-block; padding: 10px 20px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 5px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <p>Hi {fullName},</p>
            <p>We received a request to reset your password. Click the button below to reset it:</p>
            <p style='text-align: center; margin: 30px 0;'>
                <a href='{resetUrl}' class='button'>Reset Password</a>
            </p>
            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all;'>{resetUrl}</p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request a password reset, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 TON Platform. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetTicketEmailTemplate(string fullName, string eventName, string ticketCode)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f4f4f4; padding: 20px; text-align: center; }}
        .ticket-info {{ background-color: #f9f9f9; padding: 20px; margin: 20px 0; border-radius: 5px; }}
        .content {{ padding: 20px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Your Ticket is Ready!</h1>
        </div>
        <div class='content'>
            <p>Hi {fullName},</p>
            <p>Thank you for your purchase! Your ticket for <strong>{eventName}</strong> is attached to this email.</p>
            <div class='ticket-info'>
                <h3>Ticket Details</h3>
                <p><strong>Event:</strong> {eventName}</p>
                <p><strong>Ticket Code:</strong> {ticketCode}</p>
                <p><strong>QR Code:</strong> See attachment</p>
            </div>
            <p>Please present the QR code at the entrance for quick check-in.</p>
            <p>We look forward to seeing you at the event!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 TON Platform. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}