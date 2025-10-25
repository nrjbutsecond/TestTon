using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Organization
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Address
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }

        // Partnership
        public string PartnershipTier { get; set; }
        public string Status { get; set; }
        public DateTime? LicenseActiveUntil { get; set; }
        public DateTime FoundedDate { get; set; }

        // Media
        public string LogoUrl { get; set; }
        public string CoverImageUrl { get; set; }

        // Statistics
        public decimal Rating { get; set; }
        public int TotalEvents { get; set; }
        public int ActivePartners { get; set; }
        public int TotalAttendees { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Team Members
        public List<OrganizationMemberDto> Members { get; set; }

        // Meta
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }

    // List view DTO for Organization
    public class OrganizationListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public string PartnershipTier { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
        public int TotalEvents { get; set; }
        public int TotalAttendees { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public string LogoUrl { get; set; }
        public DateTime? LicenseActiveUntil { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }

    // Create Organization DTO
    public class CreateOrganizationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public DateTime FoundedDate { get; set; }
    }

    // Update Organization DTO
    public class UpdateOrganizationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
    }

    // Organization Member DTO
    public class OrganizationMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Role { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; }
    }

    // Add Member DTO
    public class AddOrganizationMemberDto
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }

    // Update Member Role DTO
    public class UpdateMemberRoleDto
    {
        public int MemberId { get; set; }
        public string Role { get; set; }
    }

    // Partnership Application DTOs
    public class PartnershipApplicationDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string RequestedTier { get; set; }
        public string ApplicationReason { get; set; }
        public string Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewNotes { get; set; }
        public string ReviewedByName { get; set; }
    }

    public class CreatePartnershipApplicationDto
    {
        public int OrganizationId { get; set; }
        public string RequestedTier { get; set; }
        public string ApplicationReason { get; set; }
    }

    public class ReviewPartnershipApplicationDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } // Approved or Rejected
        public string ReviewNotes { get; set; }
    }

    // Organization Activity DTOs
    public class OrganizationActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public string UserName { get; set; }
        public string Details { get; set; }
    }

    // Organization Statistics DTO
    public class OrganizationStatisticsDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalEvents { get; set; }
        public int TotalAttendees { get; set; }
        public decimal Revenue { get; set; }
        public int NewMembers { get; set; }
        public decimal AverageRating { get; set; }
    }

    // Dashboard DTO
    public class OrganizationDashboardDto
    {
        public OrganizationDto Organization { get; set; }
        public List<OrganizationActivityDto> RecentActivities { get; set; }
        public List<EventSummaryDto> RecentEvents { get; set; }
        public OrganizationStatisticsDto CurrentMonthStats { get; set; }
        public List<OrganizationStatisticsDto> YearlyStats { get; set; }
    }

    // Supporting DTOs
    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public int AttendeeCount { get; set; }
        public string Status { get; set; }
    }

    // Media DTOs
    public class UpdateOrganizationMediaDto
    {
        public int OrganizationId { get; set; }
        public string MediaType { get; set; } // Logo or Cover
        public string MediaUrl { get; set; }
    }

    // Filter DTO
    public class OrganizationFilterDto
    {
        public string SearchTerm { get; set; }
        public string Type { get; set; }
        public string PartnershipTier { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public decimal? MinRating { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Name";
        public bool IsDescending { get; set; } = false;
    }

    public class TalkEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public string Status { get; set; }
        public bool HasTicketSale { get; set; }
        public string BannerImage { get; set; }
        public string ThumbnailImage { get; set; }
    }

    public class MerchandiseOrganizationDto
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public string Images { get; set; }
    }
}
