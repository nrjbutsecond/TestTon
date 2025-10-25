using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sponsor
{
    public class CreateSponsorDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid URL format")]
        [MaxLength(500)]
        public string? Logo { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [MaxLength(500)]
        public string? Website { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Sponsorship level is required")]
        public string SponsorshipLevel { get; set; } = "Bronze";

        [Range(0, double.MaxValue, ErrorMessage = "Contribution amount must be positive")]
        public decimal ContributionAmount { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public List<string>? Benefits { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    // DTO for updating an existing sponsor
    public class UpdateSponsorDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid URL format")]
        [MaxLength(500)]
        public string? Logo { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [MaxLength(500)]
        public string? Website { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Sponsorship level is required")]
        public string SponsorshipLevel { get; set; } = "Bronze";

        [Range(0, double.MaxValue, ErrorMessage = "Contribution amount must be positive")]
        public decimal ContributionAmount { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public bool IsActive { get; set; }

        public List<string>? Benefits { get; set; }

        public int DisplayOrder { get; set; }
    }

    // DTO for returning sponsor information
    public class SponsorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string SponsorshipLevel { get; set; } = string.Empty;
        public decimal ContributionAmount { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsContractActive { get; set; }
        public List<string>? Benefits { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTO for public display (limited information)
    public class PublicSponsorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string SponsorshipLevel { get; set; } = string.Empty;
        public List<string>? Benefits { get; set; }
    }

    // DTO for sponsor list with pagination
    public class SponsorListDto
    {
        public List<SponsorDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // DTO for sponsor statistics
    public class SponsorStatisticsDto
    {
        public int TotalSponsors { get; set; }
        public int ActiveSponsors { get; set; }
        public int ActiveContracts { get; set; }
        public int ExpiringContracts { get; set; }
        public decimal TotalContributions { get; set; }
        public Dictionary<string, decimal> ContributionsByLevel { get; set; } = new();
        public Dictionary<string, int> CountByLevel { get; set; } = new();
    }

    // DTO for bulk operations
    public class BulkSponsorOperationDto
    {
        [Required]
        public List<int> SponsorIds { get; set; } = new();

        [Required]
        public string Operation { get; set; } = string.Empty; // "Activate", "Deactivate", "Delete"
    }

    // DTO for updating display order
    public class UpdateDisplayOrderDto
    {
        [Required]
        public Dictionary<int, int> SponsorDisplayOrders { get; set; } = new();
    }

    // DTO for search/filter parameters
    public class SponsorFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? SponsorshipLevel { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasActiveContract { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "DisplayOrder";
        public bool IsDescending { get; set; } = false;
    }
}