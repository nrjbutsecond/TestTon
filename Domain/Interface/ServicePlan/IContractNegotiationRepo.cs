using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ServicePlan
{
    public interface IContractNegotiationRepo :IRepo<ContractNegotiationModel>
    {
        Task<IEnumerable<ContractNegotiationModel>> GetPendingNegotiationsAsync();
        Task<IEnumerable<ContractNegotiationModel>> GetUserNegotiationsAsync(int userId);
        Task<IEnumerable<ContractNegotiationModel>> GetNegotiationsByStatusAsync(string status);
        Task<ContractNegotiationModel> GetNegotiationWithDetailsAsync(int negotiationId);
        Task<bool> HasPendingNegotiationAsync(int userId, int planId);
    }
}
