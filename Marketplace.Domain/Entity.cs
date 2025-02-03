using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain
{
    public class Entity<TIdentifier> : IEntity
    {
        public TIdentifier? Identifier { get; set; }
    }
}
