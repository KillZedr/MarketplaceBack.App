using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Products.Entity
{
    public class ProductImage : Entity<int>
    {
        public required int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public required string ImageUrl { get; set; }
    }
}
