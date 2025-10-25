using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SupportPersonnel
{
    // ============== Support Personnel DTOs ==============
    public class SupportPersonnelDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int RegisteredBy { get; set; }
        public string RegisteredByName { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public Dictionary<string, string> Availability { get; set; } = new();
        public bool IsActive { get; set; }
        public int ExperienceLevel { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SupportPersonnelListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public int ExperienceLevel { get; set; }
        public bool IsActive { get; set; }
        public int TotalAssignments { get; set; }
    }

    public class CreateSupportPersonnelDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<string> Skills { get; set; } = new();

        [Required]
        public Dictionary<string, string> Availability { get; set; } = new();

        [Range(1, 5)]
        public int ExperienceLevel { get; set; } = 1;

        public string? Bio { get; set; }
    }

    public class UpdateSupportPersonnelDto
    {
        public List<string>? Skills { get; set; }
        public Dictionary<string, string>? Availability { get; set; }
        public bool? IsActive { get; set; }

        [Range(1, 5)]
        public int? ExperienceLevel { get; set; }

        public string? Bio { get; set; }
    }

    // ============== Personnel Support Request DTOs ==============
    public class PersonnelSupportRequestDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string SupportType { get; set; } = string.Empty;
        public int RequiredPersonnel { get; set; }
        public Dictionary<string, object> Requirements { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public string? FulfillmentNotes { get; set; }
        public List<AssignedPersonnelDto> AssignedPersonnel { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class PersonnelSupportRequestListDto
    {
        public int Id { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public string SupportType { get; set; } = string.Empty;
        public int RequiredPersonnel { get; set; }
        public int AssignedPersonnel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePersonnelSupportRequestDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public string SupportType { get; set; } = string.Empty; // Technical|Logistics|Marketing|Other

        [Range(1, 100)]
        public int RequiredPersonnel { get; set; }

        [Required]
        public Dictionary<string, object> Requirements { get; set; } = new();

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class UpdatePersonnelSupportRequestDto
    {
        public string? SupportType { get; set; }
        public int? RequiredPersonnel { get; set; }
        public Dictionary<string, object>? Requirements { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ApproveRequestDto
    {
        [Required]
        public int RequestId { get; set; }

        public string? FulfillmentNotes { get; set; }
    }

    public class AssignPersonnelDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public List<int> PersonnelIds { get; set; } = new();

        public string? Notes { get; set; }
    }

    public class AssignedPersonnelDto
    {
        public int Id { get; set; }
        public int PersonnelId { get; set; }
        public string PersonnelName { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public DateTime AssignedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class UpdateAssignmentStatusDto
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; // Assigned|Confirmed|InProgress|Completed|Cancelled

        public string? Notes { get; set; }
    }
}