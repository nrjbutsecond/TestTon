using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using Domain.Interface.Mentor;
using Domain.Interface.OrganizationRepoFolder;
using Domain.Interface.ServicePlan;
using Infrastructure.Repo;
using Infrastructure.Repo.Mentor;
using Infrastructure.Repo.OrganizationImp;
using Infrastructure.Repo.ServicePlan;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Domain.Interface;
using Ticket.Infrastructure.Data;
namespace Ticket.Infrastructure.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        // Generic repository instances
      
       // private IRepo<ArticleModel>? _articles;
       // private IRepo<WorkshopModel>? _workshops;
      //  private IRepo<ProductModel>? _products;
        private IRepo<CartItemModel>? _cartItems;
        private IRepo<OrderItemModel>? _orderItems;
        private IRepo<ReviewVoteModel>? _reviewVotes;
        private IRepo<ReviewResponseModel>? _reviewResponse;
        private IRepo<ShippingConfig>? _shippingConfig;
        // Specific repository instances
        private IOrganizationMemberRepo? _organizationMembers;
        private IPartnershipApplicationRepo? _partnershipApplications;
        private IOrganizationActivityRepo? _organizationActivities;
        private IOrganizationStatisticsRepo? _organizationStatistics;

        private IUserRepo? _users;
        private ITicketTypeRepo? _ticketTypes;
        private ITicketRepo? _tickets;
        private IArticleTagRepo? _articleTags;
        private IWorkshopRepo? _workshops;
        private IArticleRepo? _articles;
        private ITalkEventRepo? _talkEvent;
        private ITicketScanLogRepo? _ticketScanLogs;
        private IMerchandiseRepo? _merchandiseRepo;
        private ICartRepo? _cart;
        private IOrderRepo? _order;
        private IShippingConfigRepo? _shippingConfigRepo;
       private IReviewRepo? _reviewRepo;
        private ISponsorRepo? _sponsorRepo;
        private IAdvertisementRepo? _advertisementRepo;

        private IOrganizationRepo? _organizationRepo;
        private IOrganizationMemberRepo? _organizationMemberRepo;
        private IPartnershipApplicationRepo? _partnershipApplicationRepo;
        private IOrganizationRepo? _organizationActivityRepo;
        private IOrganizationStatisticsRepo? _organizationStatisticsRepo;

        private ISupportPersonnelRepo? _supportPersonnel;
        private IPersonnelSupportRequestRepo? _personnelSupportRequest;
        private IPersonnelSupportAssignmentRepo? _personnelSupportAssignment;

        private  IServicePlanRepo? _servicePlan;
        private  IUserServicePlanSubscriptionRepo? _subscription;
        private  IContractNegotiationRepo? _negotiation;
        private IConsultationRequestRepo? _consultation;

        private ISupportTicketRepo? _supportTickets;
        private ISupportTicketMessageRepo? _supportTicketMessages;

        private IMentoringSessionRepo _mentoringSessions;
        private IMentorAvailabilityRepo _mentorAvailabilities;
        private IMentorBlockedTimeRepo _mentorBlockedTimes;
        private IMentoringParticipantRepo _mentoringParticipants;
        private IMentoringAttachmentRepo _mentoringAttachments;

      private INotificationRepo? _notification;



        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();

        }
        // Generic repositories
      //  public IRepo<ProductModel> Products => _products ??= new Repo<ProductModel>(_context);
        public IRepo<CartItemModel> CartItems => _cartItems ??= new Repo<CartItemModel>(_context);
        public IRepo<OrderItemModel> OrderItems => _orderItems ??= new Repo<OrderItemModel>(_context);

        public IRepo<ReviewVoteModel> ReviewVotes => _reviewVotes ??= new Repo<ReviewVoteModel>(_context);
        public IRepo<ReviewResponseModel> ReviewResponses => _reviewResponse ??= new Repo<ReviewResponseModel>(_context);

        public IRepo<ShippingConfig> ShippingConfigs => _shippingConfig ??= new Repo<ShippingConfig>(_context);
        // Specific repositories
        public IUserRepo Users => _users ??= new UserRepo(_context);
        public IArticleRepo Articles => _articles ??= new ArticleRepo(_context);
        public IWorkshopRepo Workshops => _workshops ??= new WorkshopRepo(_context);
        public IArticleTagRepo ArticleTags => _articleTags ??= new ArticleTagRepo(_context);
        public ITicketTypeRepo TicketTypes => _ticketTypes ??= new TicketTypeRepo(_context);
        public ITicketRepo Tickets => _tickets ??= new TicketRepo(_context);

        public IOrganizationMemberRepo OrganizationMembers => _organizationMembers ??= new OrganizationMemberRepo(_context);

        public IPartnershipApplicationRepo PartnershipApplications => _partnershipApplications ??= new PartnershipApplicationRepo(_context);

        public IOrganizationActivityRepo OrganizationActivities => _organizationActivities ??= new OrganizationActivityRepo(_context);

        public IOrganizationStatisticsRepo OrganizationStatistics => _organizationStatistics ??= new OrganizationStatisticsRepo(_context);
        IWorkshopRepo IUnitOfWork.Workshops => _workshops ??= new WorkshopRepo(_context);

        IArticleRepo IUnitOfWork.Articles => _articles ??= new ArticleRepo(_context);
        ITalkEventRepo IUnitOfWork.TalkEvent => _talkEvent ??= new TalkEventRepo(_context);

        public IShippingConfigRepo ShippingConfig => _shippingConfigRepo ??= new ShippingConfigRepo(_context);

        ITicketScanLogRepo IUnitOfWork.TicketScanLogs => _ticketScanLogs ??= new TicketScanLogRepo(_context);
        //==========merchandise==============//
        public IMerchandiseRepo Merchandises =>
          _merchandiseRepo ??= new MerchandiseRepo(_context);

        public ICartRepo Carts => _cart ??= new CartRepo(_context);
        public IOrderRepo Orders => _order ??= new OrderRepo(_context);
        public IReviewRepo Reviews => _reviewRepo ??= new ReviewRepo(_context);

        public ISponsorRepo Sponsors => _sponsorRepo ??= new SponsorRepo(_context);

        public IAdvertisementRepo Advertisements => _advertisementRepo ??= new AdvertisementRepo(_context);

        public IOrganizationRepo Organizations => _organizationRepo ??= new OrganizationRepo(_context);

        public IServicePlanRepo ServicePlan => _servicePlan ??= new ServicePlanRepo(_context);

        public IUserServicePlanSubscriptionRepo UserServicePlanSubscription => _subscription ??= new UserServicePlanSubscriptionRepo(_context);

        public IContractNegotiationRepo ContractNegotiation => _negotiation ??= new ContractNegotiationRepo(_context);

        public ISupportPersonnelRepo SupportPersonnel => _supportPersonnel ??= new SupportPersonnelRepo(_context);
        public IPersonnelSupportRequestRepo PersonnelSupportRequest => _personnelSupportRequest ??= new PersonnelSupportRequestRepo(_context);
        public IPersonnelSupportAssignmentRepo PersonnelSupportAssignment => _personnelSupportAssignment ??= new PersonnelSupportAssignmentRepo(_context);

        public IConsultationRequestRepo ConsultationRequests => _consultation ??= new ConsultationRequestRepo(_context);


        public ISupportTicketRepo SupportTickets => _supportTickets ??= new SupportTicketRepo(_context);
        public ISupportTicketMessageRepo SupportTicketMessages => _supportTicketMessages ??= new SupportTicketMessageRepo(_context);

        public IMentoringSessionRepo MentoringSessions => _mentoringSessions ??= new MentoringSessionRepo(_context);
        public IMentorAvailabilityRepo MentorAvailabilities => _mentorAvailabilities ??= new MentorAvailabilityRepo(_context);
        public IMentorBlockedTimeRepo MentorBlockedTimes => _mentorBlockedTimes ??= new MentorBlockedTimeRepo(_context);
        public IMentoringParticipantRepo MentoringParticipants => _mentoringParticipants ??= new MentoringParticipantRepo(_context);
        public IMentoringAttachmentRepo MentoringAttachments => _mentoringAttachments ??= new MentoringAttachmentRepo(_context);

        public INotificationRepo Notifications => _notification ??= new NotificationRepo(_context);


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction?.CommitAsync()!;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync()!;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public IRepo<T> Repo<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repo<T>(_context);
            }
            return (IRepo<T>)_repositories[type];
        }
    }
}