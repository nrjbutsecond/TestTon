using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class CartModel: BaseEntity
    {
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }

        public virtual UserModel User { get; set; }
        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }
}
