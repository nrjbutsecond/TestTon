using Domain.Entities.ServicePlan;
using Domain.Interface.ServicePlan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.ServicePlan
{
    public class ContractNegotiationRepo : Repo<ContractNegotiationModel>, IContractNegotiationRepo
    {
        public ContractNegotiationRepo(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<ContractNegotiationModel>> GetPendingNegotiationsAsync()
        {
            return await _context.ContractNegotiations
                .Include(cn => cn.User)
                .Include(cn => cn.ServicePlan)
                .Where(cn => cn.CurrentStatus == "Pending" && !cn.IsDeleted)
                .OrderBy(cn => cn.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContractNegotiationModel>> GetUserNegotiationsAsync(int userId)
        {
            return await _context.ContractNegotiations
                .Include(cn => cn.ServicePlan)
                .Where(cn => cn.UserId == userId && !cn.IsDeleted)
                .OrderByDescending(cn => cn.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContractNegotiationModel>> GetNegotiationsByStatusAsync(string status)
        {
            return await _context.ContractNegotiations
                .Include(cn => cn.User)
                .Include(cn => cn.ServicePlan)
                .Where(cn => cn.CurrentStatus == status && !cn.IsDeleted)
                .ToListAsync();
        }

        public async Task<ContractNegotiationModel> GetNegotiationWithDetailsAsync(int negotiationId)
        {
            return await _context.ContractNegotiations
                .Include(cn => cn.User)
                .Include(cn => cn.ServicePlan)
                .Include(cn => cn.Subscriptions)
                .FirstOrDefaultAsync(cn => cn.Id == negotiationId && !cn.IsDeleted);
        }

        public async Task<bool> HasPendingNegotiationAsync(int userId, int planId)
        {
            return await _context.ContractNegotiations
                .AnyAsync(cn => cn.UserId == userId
                    && cn.ServicePlanId == planId
                    && cn.CurrentStatus == "Pending"
                    && !cn.IsDeleted);
        }
    }
}

