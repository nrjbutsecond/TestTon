using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SponsorModel: BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Logo { get; set; }

        [MaxLength(500)]
        public string? Website { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? ContactEmail { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Required]
        [MaxLength(20)]
        public string SponsorshipLevel { get; set; } = SponsorshipLevelEnum.Bronze;

        [Range(0, double.MaxValue)]
        public decimal ContributionAmount { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Store as JSON string in database
        public string? Benefits { get; set; }

        public int DisplayOrder { get; set; } = 0;

        // Validation methods
        public bool IsContractActive()
        {
            if (!ContractStartDate.HasValue || !ContractEndDate.HasValue)
                return false;

            var now = DateTime.UtcNow;
            return now >= ContractStartDate.Value && now <= ContractEndDate.Value;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public static class SponsorshipLevelEnum
    {
        public const string Bronze = "Bronze";
        public const string Silver = "Silver";
        public const string Gold = "Gold";
        public const string Platinum = "Platinum";

        public static readonly string[] All = { Bronze, Silver, Gold, Platinum };

        public static bool IsValid(string level)
        {
            return All.Contains(level);
        }

        public static int GetPriority(string level)
        {
            return level switch
            {
                Platinum => 4,
                Gold => 3,
                Silver => 2,
                Bronze => 1,
                _ => 0
            };
        }
    }
}