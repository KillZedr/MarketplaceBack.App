using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Settings.Stripe
{
    public class StripeSetting
    {
        public string SecretKey { get; set; }
        public string WebhookSecret { get; set; }
        public string PublishableKey { get; set; }
    }
}
