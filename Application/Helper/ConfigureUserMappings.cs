using Application.DTOs.Activity;
using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        
            private void ConfigureUserMappings()
            {
                // UserModel -> UserDto
                CreateMap<UserModel, UserDtos>()
                    .ForMember(dest => dest.Role,
                        opt => opt.MapFrom(src => src.Role.ToString())); // Enum to string

            // CreateUserDto -> UserModel
            CreateMap<CreateUserDto, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Will be hashed in service
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => UserRoles.Enthusiast)) // Default role
                .ForMember(dest => dest.ServicePlan, opt => opt.MapFrom(src => ServicePlans.Free)) // Default plan
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsPartneredOrganizer, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set by BaseEntity
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlanExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizedEvents, opt => opt.Ignore())
                .ForMember(dest => dest.PurchasedTickets, opt => opt.Ignore())
                .ForMember(dest => dest.Articles, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())           
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore());

            // UpdateUserDto -> UserModel (for partial updates)
            CreateMap<UpdateUserDto, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Email cannot be changed through update
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlan, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlanExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.IsPartneredOrganizer, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizedEvents, opt => opt.Ignore())
                .ForMember(dest => dest.PurchasedTickets, opt => opt.Ignore())
                .ForMember(dest => dest.Articles, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore()); 

                // UpdateUserRoleDtoAlternative -> UserModel (for role update only)
                CreateMap<UpdateUserRoleDto, UserModel>()
                    .ForAllMembers(opt => opt.Ignore()); // Ignore all except what we explicitly map

                // UpdateUserServicePlanDto -> UserModel (for service plan update) -- will use when service plan model is ready ;-;

                CreateMap<UpdateUserServicePlanDto, UserModel>()
                    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                    {
                        // Only map these specific properties
                        var propertyName = opt.DestinationMember.Name;
                        return propertyName == nameof(UserModel.ServicePlan) ||
                               propertyName == nameof(UserModel.ServicePlanExpiry) ||
                               propertyName == nameof(UserModel.IsPartneredOrganizer);
                    }));

            CreateMap<UserModel, OrganizerDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));

            CreateMap<UserModel, AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));


        }
        }
    }
