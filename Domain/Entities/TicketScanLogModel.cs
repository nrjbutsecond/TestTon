using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TicketScanLogModel : BaseEntity
    {
        [Required]
        public int TicketId { get; set; }
        public virtual TicketModel Ticket { get; set; }

        [Required]
        public DateTime ScannedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string ScannedBy { get; set; } //email or username of the scanner
    }
}
