using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.ECommerce
{
    public class CartItem : Entity<int>
    {
        public required int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        public required int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public required int Quantity { get; set; } 
         
    }
}
