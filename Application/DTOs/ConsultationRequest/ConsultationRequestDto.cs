using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ConsultationRequest
{
    // Response DTO for listing consultations
    public class ConsultationRequestListDto
    {
        public int Id { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string ServicePlanName { get; set; } = string.Empty;
        public string ConsultationType { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AssignedStaffName { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Response DTO for consultation details
    public class ConsultationRequestDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string OrganizerEmail { get; set; } = string.Empty;
        public int ServicePlanId { get; set; }
        public string ServicePlanName { get; set; } = string.Empty;
        public string ConsultationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? AssignedStaffId { get; set; }
        public string? AssignedStaffName { get; set; }
        public string? Notes { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool CanEdit { get; set; }
        public bool CanCancel { get; set; }
    }

    // DTO for creating a new consultation request
    public class CreateConsultationRequestDto
    {
        [Required]
        public int ServicePlanId { get; set; }

        [Required]
        [EnumDataType(typeof(Domain.Entities.ServicePlan.ConsultationStatus))]
        public string ConsultationType { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PreferredDate { get; set; }

        [Required]
        [Range(1, 24, ErrorMessage = "Duration must be between 1 and 24 hours")]
        public int Duration { get; set; }
    }

    // DTO for updating consultation request (by organizer)
    public class UpdateConsultationRequestDto
    {
        [Required]
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PreferredDate { get; set; }

        [Required]
        [Range(1, 24, ErrorMessage = "Duration must be between 1 and 24 hours")]
        public int Duration { get; set; }
    }

    // DTO for staff to schedule consultation
    public class ScheduleConsultationDto
    {
        [Required]
        public int AssignedStaffId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ScheduledDate { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }

    // DTO for updating consultation status
    public class UpdateConsultationStatusDto
    {
        [Required]
        [EnumDataType(typeof(Domain.Entities.ServicePlan.ConsultationStatus))]
        public string Status { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }
    }

    // DTO for consultation statistics
    public class ConsultationStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ScheduledRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public double AverageDuration { get; set; }
        public Dictionary<string, int> RequestsByType { get; set; } = new();
    }
}