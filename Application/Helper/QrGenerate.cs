using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

namespace Ticket.Application.Helper
{
    public class QrGenerate
    {
        public static string GenerateQrAsBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrBytes = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrBytes);
        }
    }
}
