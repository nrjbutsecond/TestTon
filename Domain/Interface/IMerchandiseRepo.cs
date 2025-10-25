using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IMerchandiseRepo: IRepo<Merchandise>
    {
        Task<IEnumerable<Merchandise>> GetActiveProductsAsync(int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<Merchandise>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<Merchandise>> GetOfficialProductsAsync();
        Task<Merchandise?> GetBySkuAsync(string sku);
        Task<Merchandise?> GetWithDetailsAsync(int id);
        Task<IEnumerable<Merchandise>> SearchAsync(string searchTerm);
        Task<IEnumerable<Merchandise>> GetByCategoryAsync(string category);
    }
}
