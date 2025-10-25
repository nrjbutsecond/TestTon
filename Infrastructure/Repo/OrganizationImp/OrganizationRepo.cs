using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Entities.Organize;
using Domain.Interface.OrganizationRepoFolder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.OrganizationImp
{
    public class OrganizationRepo : Repo<OrganizationModel>, IOrganizationRepo
    {

        public OrganizationRepo(AppDbContext context) : base(context) { }

        public async Task<OrganizationModel> GetOrganizationWithDetailsAsync(int id)
        {
            return await _context.Organizations
                .Include(o => o.Members)
                    .ThenInclude(m => m.User)
                .Include(o => o.Applications)
                .Include(o => o.Activities)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task<OrganizationModel> GetOrganizationWithMembersAsync(int id)
        {
            return await _context.Organizations
                .Include(o => o.Members.Where(m => !m.IsDeleted && m.IsActive))
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        // Get TalkEvents through OrganizationMember relationship
        public async Task<IEnumerable<TalkEventModel>> GetOrganizationEventsAsync(int organizationId)
        {
            var events = await (
                from e in _context.TalkEvents
                join u in _context.Users on e.OrganizerId equals u.Id
                join m in _context.OrganizationMembers on u.Id equals m.UserId
                where m.OrganizationId == organizationId
                    && !m.IsDeleted
                    && m.IsActive
                    && !e.IsDeleted
                    && (m.Role == OrganizationRole.Organizer || m.Role == OrganizationRole.Admin)
                select e
            ).Include(e => e.TicketTypes)
             .ToListAsync();

            return events;
        }

        // Get Merchandise through OrganizationMember relationship
        public async Task<IEnumerable<Merchandise>> GetOrganizationMerchandiseAsync(int organizationId)
        {
            var merchandise = await (
                from merch in _context.Merchandises
                join u in _context.Users on merch.SellerId equals u.Id
                join m in _context.OrganizationMembers on u.Id equals m.UserId
                where m.OrganizationId == organizationId
                    && !m.IsDeleted
                    && m.IsActive
                    && !merch.IsDeleted
                    && !merch.IsOfficial // Only organization merchandise, not official
                select merch
            ).Include(m => m.Variants)
             .ToListAsync();

            return merchandise;
        }

        // Count events for organization
        public async Task<int> GetOrganizationEventCountAsync(int organizationId)
        {
            return await (
                from e in _context.TalkEvents
                join m in _context.OrganizationMembers on e.OrganizerId equals m.UserId
                where m.OrganizationId == organizationId
                    && !m.IsDeleted
                    && !e.IsDeleted
                select e
            ).CountAsync();
        }

        // Calculate organization revenue
        public async Task<decimal> GetOrganizationRevenueAsync(int organizationId, DateTime startDate, DateTime endDate)
        {
            // Revenue from event tickets
            var ticketRevenue = await (
                from t in _context.Tickets
                join tt in _context.TicketTypes on t.TicketTypeId equals tt.Id
                join e in _context.TalkEvents on tt.TalkEventId equals e.Id
                join m in _context.OrganizationMembers on e.OrganizerId equals m.UserId
                where m.OrganizationId == organizationId
                    && t.PurchaseDate >= startDate
                    && t.PurchaseDate <= endDate
                    && t.Status != TicketStatus.Cancelled
                    && !t.IsDeleted
                    && !m.IsDeleted
                select tt.Price
            ).SumAsync();

            // Revenue from merchandise
            var merchRevenue = await (
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join merch in _context.Merchandises on oi.MerchandiseId equals merch.Id
                join m in _context.OrganizationMembers on merch.SellerId equals m.UserId
                where m.OrganizationId == organizationId
                    && o.CreatedAt >= startDate
                    && o.CreatedAt <= endDate
                    && o.Status != OrderStatus.Cancelled
                    && !o.IsDeleted
                    && !m.IsDeleted
                select oi.TotalPrice
            ).SumAsync();

            return ticketRevenue + merchRevenue;
        }

        // Update organization statistics
        public async Task UpdateStatisticsAsync(int organizationId)
        {
            var org = await GetByIdAsync(organizationId);
            if (org == null) return;

            // Total events through members
            org.TotalEvents = await GetOrganizationEventCountAsync(organizationId);

            // Total attendees through event tickets
            org.TotalAttendees = await (
                from t in _context.Tickets
                join e in _context.TalkEvents on t.TicketableId equals e.Id
                join m in _context.OrganizationMembers on e.OrganizerId equals m.UserId
                where m.OrganizationId == organizationId
                    && t.TicketableType == TicketableTypes.TalkEvent
                    && t.Status == TicketStatus.Used
                    && !t.IsDeleted
                    && !m.IsDeleted
                    && !e.IsDeleted
                select t
            ).CountAsync();

            // Active members count
            org.ActivePartners = await _context.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId && m.IsActive && !m.IsDeleted);

            // Monthly revenue
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            org.MonthlyRevenue = await GetOrganizationRevenueAsync(organizationId, startOfMonth, DateTime.UtcNow);

            Update(org);
            await _context.SaveChangesAsync();
        }

        // Check if organization has active events
        public async Task<bool> HasActiveEventsAsync(int organizationId)
        {
            return await (
                from e in _context.TalkEvents
                join m in _context.OrganizationMembers on e.OrganizerId equals m.UserId
                where m.OrganizationId == organizationId
                    && !m.IsDeleted
                    && !e.IsDeleted
                    && e.Status != TalkEventStatus.Cancelled
                    && e.Status != TalkEventStatus.Completed
                select e
            ).AnyAsync();
        }

        // Check if organization has active merchandise
        public async Task<bool> HasActiveMerchandiseAsync(int organizationId)
        {
            return await (
                from merch in _context.Merchandises
                join m in _context.OrganizationMembers on merch.SellerId equals m.UserId
                where m.OrganizationId == organizationId
                    && !m.IsDeleted
                    && !merch.IsDeleted
                    && merch.IsActive
                select merch
            ).AnyAsync();
        }

        // Other basic methods
        public async Task<IEnumerable<OrganizationModel>> GetActivePartnersAsync()
        {
            return await _context.Organizations
                .Where(o => !o.IsDeleted
                    && o.Status == PartnershipStatus.Active
                    && o.PartnershipTier != PartnershipTier.None
                    && o.LicenseActiveUntil > DateTime.UtcNow)
                .OrderByDescending(o => o.PartnershipTier)
                .ThenBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrganizationModel>> GetFeaturedOrganizationsAsync()
        {
            return await _context.Organizations
                .Where(o => !o.IsDeleted
                    && o.Type == OrganizationType.Featured
                    && o.Status == PartnershipStatus.Active)
                .OrderByDescending(o => o.Rating)
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Organizations.Where(o => o.Name == name && !o.IsDeleted);
            if (excludeId.HasValue)
                query = query.Where(o => o.Id != excludeId.Value);
            return !await query.AnyAsync();
        }

    }

}