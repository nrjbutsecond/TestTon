using Application.DTOs.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Activity
{
    public class WorkshopDto :BaseDto
    {


        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? OnlineLink { get; set; }
        public bool IsOfficial { get; set; }
        public OrganizerDto? Organizer { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public int AvailableSlots => MaxParticipants - CurrentParticipants;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;
        public bool CanRegister => AvailableSlots > 0 &&
                                   RegistrationDeadline > DateTime.UtcNow &&
                                   Status == WorkshopStatus.Published;
    }

    public class WorkshopListDto : BaseDto  // ADD INHERITANCE
    {
        // Remove duplicate properties
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsOfficial { get; set; }
        public decimal Price { get; set; }
        public int AvailableSlots { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
    }

    public class CreateWorkshopDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? OnlineLink { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime RegistrationDeadline { get; set; }
    }

    public class UpdateWorkshopDto : CreateWorkshopDto
    {
        public int Id { get; set; }
        public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;
    }

    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

