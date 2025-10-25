using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class UserOrderStatistics
    {
        public int UserId {  get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalCost { get; set; }


    }
}
