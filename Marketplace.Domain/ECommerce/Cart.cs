using Marketplace.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.ECommerce
{
    public class Cart : Entity<int>
    {
        public required string UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
        public bool IsPaid { get; set; } = false; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public virtual PaidCart? PaidCart { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
