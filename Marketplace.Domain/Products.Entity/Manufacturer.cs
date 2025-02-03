using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Products.Entity
{
    public class Manufacturer : Entity<int>
    {
        public required string Name { get; set; } 
        public string? Country { get; set; }     
        public string? Website { get; set; }     
        public virtual IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
