using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.PaymentTransactions
{
    public class ProcessedStripeEvent : Entity<int>
    {
        public string EventId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
