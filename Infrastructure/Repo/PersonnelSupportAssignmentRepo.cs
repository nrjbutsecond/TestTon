using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class PersonnelSupportAssignmentRepo : Repo<PersonnelSupportAssignment>, IPersonnelSupportAssignmentRepo
    {
        public PersonnelSupportAssignmentRepo(AppDbContext context) : base(context) { }
        
public async Task<PersonnelSupportAssignment?> GetByRequestAndPersonnelAsync(int requestId, int personnelId)
        {
            return await _context.PersonnelSupportAssignments
                .Include(a => a.Request)
                .Include(a => a.Personnel)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a =>
                    a.PersonnelSupportRequestId == requestId &&
                    a.SupportPersonnelId == personnelId &&
                    !a.IsDeleted);
        }

        public async Task<IEnumerable<PersonnelSupportAssignment>> GetByRequestIdAsync(int requestId)
        {
            return await _context.PersonnelSupportAssignments
                .Include(a => a.Personnel)
                    .ThenInclude(p => p.User)
                .Where(a => a.PersonnelSupportRequestId == requestId && !a.IsDeleted)
                .OrderBy(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonnelSupportAssignment>> GetByPersonnelIdAsync(int personnelId)
        {
            return await _context.PersonnelSupportAssignments
                .Include(a => a.Request)
                    .ThenInclude(r => r.Event)
                .Where(a => a.SupportPersonnelId == personnelId && !a.IsDeleted)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<bool> IsPersonnelAssignedAsync(int requestId, int personnelId)
        {
            return await _context.PersonnelSupportAssignments
                .AnyAsync(a =>
                    a.PersonnelSupportRequestId == requestId &&
                    a.SupportPersonnelId == personnelId &&
                    !a.IsDeleted);
        }

        public async Task<int> GetAssignmentCountByRequestAsync(int requestId)
        {
            return await _context.PersonnelSupportAssignments
                .CountAsync(a =>
                    a.PersonnelSupportRequestId == requestId &&
                    !a.IsDeleted);
        }
    }
}