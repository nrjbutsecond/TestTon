using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IMerchandiseVariantRepo : IRepo<MerchandiseVariant>
    {
        Task<IEnumerable<MerchandiseVariant>> GetByMerchandiseIdAsync(int merchandiseId);
        Task<MerchandiseVariant?> GetWithMerchandiseAsync(int variantId);
    }
}
