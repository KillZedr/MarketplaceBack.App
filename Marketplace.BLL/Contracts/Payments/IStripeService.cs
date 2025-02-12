using Marketplace.Domain.ECommerce;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Payments
{
    public interface IStripeService : IService
    {
        Task<string> CreatePaymentLinkAsync(int idUserCart);
        Task ProcessStripeWebhookAsync(Event stripeEvent);
        Task<bool> RefundPaymentAsync(string paymentIntentId);

    }
}
