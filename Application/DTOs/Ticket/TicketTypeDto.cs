using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ticket
{
    public class TicketTypeDto
    {
        public int Id { get; set; }
        public int? TalkEventId { get; set; }
        public int? WorkshopId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MaxQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public List<string>? Benefits { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsSoldOut { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Event details
        public string? EventTitle { get; set; }
        public string EventType { get; set; } // "TalkEvent" or "Workshop"
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? EventLocation { get; set; }
    }
    public class CreateTicketTypeForTalkEventDto
    {
        [Required]
        public int TalkEventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxQuantity { get; set; }

        public List<string>? Benefits { get; set; }

        [Required]
        public DateTime SaleStartDate { get; set; }

        [Required]
        public DateTime SaleEndDate { get; set; }
    }

    public class CreateTicketTypeForWorkshopDto
    {
        [Required]
        public int WorkshopId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxQuantity { get; set; }

        public List<string>? Benefits { get; set; }

        [Required]
        public DateTime SaleStartDate { get; set; }

        [Required]
        public DateTime SaleEndDate { get; set; }
    }

    public class UpdateTicketTypeDto : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxQuantity { get; set; }

        public List<string>? Benefits { get; set; }

        [Required]
        public DateTime SaleStartDate { get; set; }

        [Required]
        public DateTime SaleEndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SaleEndDate <= SaleStartDate)
                yield return new ValidationResult("Sale end date must be after sale start date",
                    new[] { nameof(SaleEndDate) });
        }
    }

    public class TicketTypeListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsSoldOut { get; set; }
        public string EventTitle { get; set; }
        public string EventType { get; set; }
    }
    public class UpdateSoldQuantityRequest
    {
        public int Quantity { get; set; }
        public bool IsIncrement { get; set; }
    }

    public class AvailabilityResponse
    {
        public int TicketTypeId { get; set; }
        public int RequestedQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RevenueResponse
    {
        public int TicketTypeId { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TotalSoldResponse
    {
        public int EventId { get; set; }
        public string EventType { get; set; }
        public int TotalSold { get; set; }
    }
}