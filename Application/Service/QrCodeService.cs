using Domain.Entities;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ticket.Application.Helper;
using Ticket.Domain.Interface;
using System.Text.Json;
namespace Ticket.Application.Service
{

    public class QrCodeResult
    {
        public string EncryptedData { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class QrValidationResult
    {
        public bool IsValid { get; set; }
        public int? TicketId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class QrCodeSettings
    {
        public string SecretKey { get; set; } = "DefaultSecretKey123!@#MustBe32Ch";
        public int QrCodeSize { get; set; } = 20;
    }

    public class QrCodeService : IQrCodeService
    {
        private readonly QrCodeSettings _settings;

        public QrCodeService(QrCodeSettings settings)
        {
            _settings = settings;
        }

        public async Task<QrCodeResult> GenerateQrCodeAsync(TicketModel ticket)
        {
            // Create data to encode
            var qrData = new
            {
                TicketId = ticket.Id,
                TicketCode = ticket.Guid,
                TalkEventId = ticket.TicketType?.TalkEventId,
                WorkshopId = ticket.TicketType?.WorkshopId,
                Timestamp = DateTime.UtcNow.Ticks
            };

            var jsonData = JsonSerializer.Serialize(qrData);
            var encryptedData = Encrypt(jsonData);

            // Generate QR code using BitmapByteQRCode (works with .NET 8)
            string base64Image;
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(encryptedData, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new BitmapByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(_settings.QrCodeSize);
                        base64Image = Convert.ToBase64String(qrCodeImage);
                    }
                }
            }

            return await Task.FromResult(new QrCodeResult
            {
                EncryptedData = encryptedData,
                ImageUrl = $"data:image/png;base64,{base64Image}"
            });
        }

        public async Task<QrValidationResult> ValidateQrCodeAsync(string qrData)
        {
            try
            {
                var decryptedData = Decrypt(qrData);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                using var doc = JsonDocument.Parse(decryptedData);
                var root = doc.RootElement;

                if (!root.TryGetProperty("TicketId", out var ticketIdElement))
                {
                    return new QrValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid QR code format - missing TicketId"
                    };
                }

                var ticketId = ticketIdElement.GetInt32();

                // Validate timestamp if exists
                if (root.TryGetProperty("Timestamp", out var timestampElement))
                {
                    var timestamp = new DateTime(timestampElement.GetInt64());
                    if (DateTime.UtcNow.Subtract(timestamp).TotalDays > 365)
                    {
                        return new QrValidationResult
                        {
                            IsValid = false,
                            ErrorMessage = "QR code has expired"
                        };
                    }
                }

                return await Task.FromResult(new QrValidationResult
                {
                    IsValid = true,
                    TicketId = ticketId
                });
            }
            catch (Exception ex)
            {
                return new QrValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Failed to decode QR code: {ex.Message}"
                };
            }
        }

        private string Encrypt(string plainText)
        {
            using var aes = Aes.Create();

            // Ensure key is exactly 32 bytes
            var keyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
            Array.Resize(ref keyBytes, 32);
            aes.Key = keyBytes;

            // Use a fixed IV for simplicity (in production, use random IV and prepend to ciphertext)
            aes.IV = new byte[16];

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        private string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();

            // Ensure key is exactly 32 bytes
            var keyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
            Array.Resize(ref keyBytes, 32);
            aes.Key = keyBytes;

            // Same IV as encryption
            aes.IV = new byte[16];

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}