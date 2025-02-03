using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Products.Entity
{
    public class Product : Entity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public decimal? Price { get; set; }

        public int Stock { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public int ReviewsCount { get; set; } = 0;
        public decimal? DiscountPrice { get; set; }
        public decimal FinalPrice => DiscountPrice ?? Price ?? 0;

        public virtual IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual IEnumerable<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    }
}
