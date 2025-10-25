using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TalkEvent
{

    public class CreateTalkEventDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public int MaxAttendees { get; set; }

        public bool HasTicketSale { get; set; }
        public string? BannerImage { get; set; }
        public string? ThumbnailImage { get; set; }
    }

    public class UpdateTalkEventDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public int MaxAttendees { get; set; }

        public bool HasTicketSale { get; set; }
        public string? BannerImage { get; set; }
        public string? ThumbnailImage { get; set; }
    }

    public class TalkEventResponseDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public bool IsPartneredOrganizer { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }
        public bool HasTicketSale { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BannerImage { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TalkEventListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ThumbnailImage { get; set; }
        public bool IsPartneredOrganizer { get; set; }
    }

    public class DeletedTalkEventDto : TalkEventListDto
    {
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public int DaysSinceDeleted => DeletedAt.HasValue
            ? (DateTime.UtcNow - DeletedAt.Value).Days
            : 0;
        public bool CanRestore { get; set; }
        public string? RestoreBlockReason { get; set; }
        public int TicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }


    public class DeletedEventFilterDto
    {
        public string? SearchTerm { get; set; }
        public DateTime? DeletedFrom { get; set; }
        public DateTime? DeletedTo { get; set; }
        public bool? HasTicketsSold { get; set; }
        public string? OrderBy { get; set; } = "deleted";
    }

    public class RestoreValidationDto
    {
        public bool CanRestore { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public TalkEventResponseDto? EventDetails { get; set; }
    }

    public class RestoreEventDto
    {
        public string? Notes { get; set; }
    }
}
