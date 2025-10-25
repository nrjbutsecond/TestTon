using Domain.Entities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Ticket.Application.Service;

namespace Ticket.Domain.Interface
{
    public interface IQrCodeService
    {
        Task<QrCodeResult> GenerateQrCodeAsync(TicketModel ticket);
        Task<QrValidationResult> ValidateQrCodeAsync(string qrData);
    }

}