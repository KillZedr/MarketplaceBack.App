using Marketplace.Domain.PaymentTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Notifications
{
    public interface IEmailNotificationService : IService
    {
        Task SendRefundNotificationEmailForStripe(StripePaymentTransaction transaction);
        Task SendSuccessNotificationAsync(string email, decimal amount, string currency, DateTime paymentTime);
        Task SendErrorNotificationAsync(string toEmail, string paymentId, string errorMessage);
        Task SendInsufficientFundsNotificationAsync(string toEmail, string paymentId);
    }
}
