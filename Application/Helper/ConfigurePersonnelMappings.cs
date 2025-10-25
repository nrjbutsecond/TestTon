using Application.DTOs.SupportPersonnel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigurePersonnelMappings()
        {
            // ============== Support Personnel Mappings ==============
            CreateMap<SupportPersonnel, SupportPersonnelDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""))
                .ForMember(dest => dest.RegisteredByName, opt => opt.MapFrom(src => src.RegisteredByUser != null ? src.RegisteredByUser.FullName : "Unknown"))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => DeserializeList(src.Skills)))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => DeserializeDictionary(src.Availability)));

            CreateMap<SupportPersonnel, SupportPersonnelListDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => DeserializeList(src.Skills)))
                .ForMember(dest => dest.TotalAssignments, opt => opt.MapFrom(src => src.Assignments != null ? src.Assignments.Count : 0));

            CreateMap<CreateSupportPersonnelDto, SupportPersonnel>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => SerializeList(src.Skills)))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => SerializeDictionary(src.Availability)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateSupportPersonnelDto, SupportPersonnel>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom((src, dest) => src.Skills != null ? SerializeList(src.Skills) : dest.Skills))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom((src, dest) => src.Availability != null ? SerializeDictionary(src.Availability) : dest.Availability))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive ?? dest.IsActive))
                .ForMember(dest => dest.ExperienceLevel, opt => opt.MapFrom((src, dest) => src.ExperienceLevel ?? dest.ExperienceLevel))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom((src, dest) => src.Bio ?? dest.Bio))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ============== Personnel Support Request Mappings ==============
            CreateMap<PersonnelSupportRequest, PersonnelSupportRequestDto>()
                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : "Unknown"))
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event != null ? src.Event.Title : "Unknown"))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByUser != null ? src.ApprovedByUser.FullName : null))
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => DeserializeDictionary(src.Requirements)))
                .ForMember(dest => dest.AssignedPersonnel, opt => opt.MapFrom(src => src.Assignments));

            CreateMap<PersonnelSupportRequest, PersonnelSupportRequestListDto>()
                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : "Unknown"))
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event != null ? src.Event.Title : "Unknown"))
                .ForMember(dest => dest.AssignedPersonnel, opt => opt.MapFrom(src => src.Assignments != null ? src.Assignments.Count : 0));

            CreateMap<CreatePersonnelSupportRequestDto, PersonnelSupportRequest>()
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => SerializeDictionary(src.Requirements)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));

            CreateMap<UpdatePersonnelSupportRequestDto, PersonnelSupportRequest>()
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom((src, dest) => src.Requirements != null ? SerializeDictionary(src.Requirements) : dest.Requirements))
                .ForMember(dest => dest.SupportType, opt => opt.MapFrom((src, dest) => src.SupportType ?? dest.SupportType))
                .ForMember(dest => dest.RequiredPersonnel, opt => opt.MapFrom((src, dest) => src.RequiredPersonnel ?? dest.RequiredPersonnel))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom((src, dest) => src.StartDate ?? dest.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom((src, dest) => src.EndDate ?? dest.EndDate))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ============== Assignment Mappings ==============
            CreateMap<PersonnelSupportAssignment, AssignedPersonnelDto>()
                .ForMember(dest => dest.PersonnelId, opt => opt.MapFrom(src => src.SupportPersonnelId))
                .ForMember(dest => dest.PersonnelName, opt => opt.MapFrom(src => src.Personnel != null && src.Personnel.User != null ? src.Personnel.User.FullName : "Unknown"))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Personnel != null ? DeserializeList(src.Personnel.Skills) : new List<string>()));
        }

        // Helper methods for JSON serialization
        private static List<string> DeserializeList(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static Dictionary<string, string> DeserializeDictionary(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, string>();

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        private static Dictionary<string, object> DeserializeDictionaryObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, object>();

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        private static string SerializeList(List<string> list)
        {
            return JsonSerializer.Serialize(list);
        }

        private static string SerializeDictionary<T>(Dictionary<string, T> dict)
        {
            return JsonSerializer.Serialize(dict);
        }
    }
}