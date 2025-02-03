using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.ECommerce
{
    public class PaidCart : Entity<int>
    {
        public required int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        public required DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public required decimal TotalAmount { get; set; } 
        public string PaymentMethod { get; set; } = string.Empty; 
        public string TransactionId { get; set; } = string.Empty; 
    }
}
