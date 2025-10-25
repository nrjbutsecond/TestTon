using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface.Mentor;
using Domain.Interface.OrganizationRepoFolder;
using Domain.Interface.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Domain.Interface;
namespace Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
       // IRepo<ArticleModel> Articles { get; }
       // IRepo<WorkshopModel> Workshops { get; }
        IWorkshopRepo Workshops { get; }
       // IRepo<ProductModel> Products { get; } //haven't done
        IRepo<CartItemModel> CartItems { get; }

        IRepo<OrderItemModel> OrderItems { get; }

        IRepo<ReviewVoteModel> ReviewVotes { get; }
        IRepo<ReviewResponseModel> ReviewResponses { get; }
        IRepo<ShippingConfig> ShippingConfigs {  get; }
        //  IRepo<ArticleTagModel> ArticleTag { get; }
        IArticleTagRepo ArticleTags { get; }

        // Specific repositories - Use specific interfaces

        IOrganizationRepo Organizations { get; }
        IOrganizationMemberRepo OrganizationMembers {  get; }
        IPartnershipApplicationRepo PartnershipApplications { get; }
        IOrganizationActivityRepo OrganizationActivities { get; }
        IOrganizationStatisticsRepo OrganizationStatistics { get; }


        ITicketTypeRepo TicketTypes { get; }
        ITicketRepo Tickets { get; }
        ITalkEventRepo TalkEvent { get; }
        IArticleRepo Articles{ get; }
        IUserRepo Users { get; }
        ITicketScanLogRepo TicketScanLogs { get; }
        IMerchandiseRepo Merchandises { get; }
        ICartRepo Carts { get; }
        IOrderRepo Orders { get; }
        IShippingConfigRepo ShippingConfig { get; }
        IReviewRepo Reviews { get; }

        ISponsorRepo Sponsors { get; }

        IAdvertisementRepo Advertisements { get; }

        ISupportPersonnelRepo SupportPersonnel { get; }
        IPersonnelSupportRequestRepo PersonnelSupportRequest { get; }
        IPersonnelSupportAssignmentRepo PersonnelSupportAssignment { get; }

        IServicePlanRepo ServicePlan {  get; }
        IUserServicePlanSubscriptionRepo UserServicePlanSubscription { get; }
        IContractNegotiationRepo ContractNegotiation {  get; }

        IConsultationRequestRepo ConsultationRequests {  get; }

        ISupportTicketRepo SupportTickets { get; }
        ISupportTicketMessageRepo SupportTicketMessages { get; }

        IMentoringSessionRepo MentoringSessions { get; }
        IMentorAvailabilityRepo MentorAvailabilities { get; }
        IMentorBlockedTimeRepo MentorBlockedTimes { get; }
        IMentoringParticipantRepo MentoringParticipants { get; }
        IMentoringAttachmentRepo MentoringAttachments { get; }

        INotificationRepo Notifications { get; }


        IRepo<T> Repo<T>() where T : class;
        // Transaction management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}