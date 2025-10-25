using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ServicePlan
{
    [Table("ConsultationRequests")]
    public class ConsultationRequest : BaseEntity
    {
        [Required]
        public int OrganizerId { get; set; }

        [Required]
        public int ServicePlanId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConsultationType ConsultationType { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime PreferredDate { get; set; }

        [Required]
        [Range(1, 24)]
        public int Duration { get; set; } // in hours

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Pending;

        public int? AssignedStaffId { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(OrganizerId))]
        public virtual UserModel? Organizer { get; set; }

        [ForeignKey(nameof(ServicePlanId))]
        public virtual ServicePlanModel? ServicePlan { get; set; }

        [ForeignKey(nameof(AssignedStaffId))]
        public virtual UserModel? AssignedStaff { get; set; }

       // public virtual ICollection<MentoringRecord> MentoringRecords { get; set; } = new List<MentoringRecord>();
    }

    public enum ConsultationType
    {
        EventPlanning,
        Mentoring,
        Personnel,
        Custom
    }

    public enum ConsultationStatus
    {
        Pending,
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

}
