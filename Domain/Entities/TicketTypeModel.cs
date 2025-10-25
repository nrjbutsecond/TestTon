using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TicketTypeModel : BaseEntity
    {
        public int? TalkEventId { get; set; }
        public virtual TalkEventModel? TalkEvent { get; set; }

        public int? WorkshopId { get; set; }
        public virtual WorkshopModel? Workshop { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int SoldQuantity { get; set; } = 0;

        public string? Benefits { get; set; } // JSON array

        [Required]
        public DateTime SaleStartDate { get; set; }

        [Required]
        public DateTime SaleEndDate { get; set; }

        public string? ReasonDelete { get; set; }

        // Navigation
        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();

        // Computed properties
        public int AvailableQuantity => MaxQuantity - SoldQuantity;
        public bool IsOnSale => DateTime.UtcNow >= SaleStartDate && DateTime.UtcNow <= SaleEndDate;
        public bool IsSoldOut => SoldQuantity >= MaxQuantity;
    }
}