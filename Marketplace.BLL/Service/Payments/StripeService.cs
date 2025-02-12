using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.ECommerce;
using Marketplace.BLL.Contracts.Notifications;
using Marketplace.BLL.Contracts.Payments;
using Marketplace.BLL.Settings.Stripe;
using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Identity;
using Marketplace.Domain.PaymentTransactions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Forwarding;
using Stripe.Issuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Payments
{
    public class StripeService : IStripeService
    {
        private readonly StripeSetting _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeService> _logger;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ICartService _cartService;

        public StripeService
            (
                IOptions<StripeSetting> options,
                IUnitOfWork unitOfWork,
                ILogger<StripeService> logger,
                IEmailNotificationService emailNotificationService,
                ICartService cartService
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _settings = options.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey;
            _emailNotificationService = emailNotificationService;
            _cartService = cartService;

        }

        public async Task<string> CreatePaymentLinkAsync(int idUserCart)
        {

            var findCart = await _unitOfWork.GetRepository<Cart>().AsQueryable().FirstOrDefaultAsync(c => c.Identifier == idUserCart);
            if (findCart == null)
            {
                _logger.LogWarning($"Cart with ID {idUserCart} not found.");
                throw new KeyNotFoundException("Cart not found.");
            }
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "sepa", "revolut_pay" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = "https://yourdomain.com/success",
                CancelUrl = "https://yourdomain.com/cancel",
                Metadata = new Dictionary<string, string>
                {
                        { "CartId", idUserCart.ToString() },
                        { "UserId", findCart.UserId }
                }
            };

            foreach (var item in findCart.CartItems)
            {
                if (item.Product == null)
                {
                    _logger.LogWarning($"Cart item {item.Identifier} has no associated product.");
                    continue;
                }
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(item.Product.FinalPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Quantity
                });
            }
            if (options.LineItems.Count == 0)
            {
                _logger.LogWarning("No valid items found in the cart.");
                throw new InvalidOperationException("Cart has no valid items for payment.");
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task ProcessStripeWebhookAsync(Event stripeEvent)
        {
            // Проверяем, обрабатывали ли это событие ранее
            var isEventProcessed = await _unitOfWork.GetRepository<ProcessedStripeEvent>()
                .AsQueryable()
                .AnyAsync(e => e.EventId == stripeEvent.Id);

            if (isEventProcessed)
            {
                _logger.LogInformation($"Повторное событие {stripeEvent.Id} пропущено.");
                return; // Пропускаем дубликат
            }

            try
            {
                // Добавляем событие в список обработанных перед обработкой
                var processedEvent = new ProcessedStripeEvent
                {
                    EventId = stripeEvent.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.GetRepository<ProcessedStripeEvent>().Create(processedEvent);
                await _unitOfWork.SaveChangesAsync();

                // Обработка событий Stripe
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;

                    case "payment_intent.succeeded":
                        await HandlePaymentIntentSucceeded(stripeEvent);
                        break;

                    case "payment_intent.payment_failed":
                        await HandlePaymentIntentFailed(stripeEvent);
                        break;

                    case "charge.succeeded":
                        await HandleChargeSucceeded(stripeEvent);
                        break;

                    case "charge.updated":
                        await HandleChargeUpdated(stripeEvent);
                        break;

                    case "payment_intent.requires_action":
                        _logger.LogInformation($"Payment Intent требует дополнительных действий: {stripeEvent.Id}");
                        break;

                    default:
                        _logger.LogWarning($"Необработанное событие Stripe: {stripeEvent.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка обработки события {stripeEvent.Id}: {ex.Message}");
            }
        }

        public async Task<bool> RefundPaymentAsync(string paymentIntentId)
        {
            try
            {
                var options = new RefundCreateOptions { PaymentIntent = paymentIntentId };
                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                if (refund.Status == "succeeded")
                {
                    await _emailNotificationService.SendRefundNotificationEmailForStripe(new StripePaymentTransaction
                    {
                        PaymentIntentId = paymentIntentId,
                        Amount = refund.Amount,
                        Currency = refund.Currency,
                        CustomerEmail = "customer@example.com"
                    });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Refund failed: {ex.Message}");
                return false;
            }
        }





        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            var transaction = new StripePaymentTransaction
            {
                StripeSessionId = session.Id,
                PaymentIntentId = session.PaymentIntentId,
                PaymentStatus = session.PaymentStatus,
                Amount = session.AmountTotal ?? 0,
                Currency = session.Currency,
                CustomerEmail = session.CustomerEmail,
                CustomerId = session.CustomerId,
                CreatedAt = DateTime.UtcNow,
                UserId = session.Metadata["UserId"],
                CartId = session.Metadata["CartId"]
            };

            _unitOfWork.GetRepository<StripePaymentTransaction>().Create(transaction);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Checkout session completed: {session.Id}");
        }

        private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var existingTransaction = await _unitOfWork.GetRepository<StripePaymentTransaction>()
                .AsQueryable()
                .FirstOrDefaultAsync(t => t.PaymentIntentId == paymentIntent.Id);

            if (existingTransaction != null)
            {
                existingTransaction.PaymentStatus = paymentIntent.Status;
                existingTransaction.Amount = paymentIntent.Amount;
                existingTransaction.Currency = paymentIntent.Currency;
                existingTransaction.CustomerEmail = paymentIntent.ReceiptEmail;
                existingTransaction.CustomerId = paymentIntent.CustomerId;
                existingTransaction.InvoiceId = paymentIntent.InvoiceId;
                existingTransaction.UpdatedAt = DateTime.UtcNow;

                existingTransaction.UserId = existingTransaction?.UserId;
                existingTransaction.CartId = existingTransaction?.CartId;

                _unitOfWork.GetRepository<StripePaymentTransaction>().Update(existingTransaction);
            }
            else
            {
                var newTransaction = new StripePaymentTransaction
                {
                    PaymentIntentId = paymentIntent.Id,
                    PaymentStatus = paymentIntent.Status,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    CreatedAt = DateTime.UtcNow,
                    CustomerEmail = paymentIntent.ReceiptEmail,
                    CustomerId = paymentIntent.CustomerId,
                    InvoiceId = paymentIntent.InvoiceId,
                    UserId = paymentIntent.Metadata.ContainsKey("UserId") ? paymentIntent.Metadata["UserId"] : null,
                    CartId = paymentIntent.Metadata.ContainsKey("CartId") ? paymentIntent.Metadata["CartId"] : null
                };
                _unitOfWork.GetRepository<StripePaymentTransaction>().Create(newTransaction);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Payment Intent succeeded: {paymentIntent.Id}");
        }

        private async Task HandlePaymentIntentFailed(Event stripeEvent)
        {
            var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (failedPaymentIntent == null) return;

            var existingTransaction = await _unitOfWork.GetRepository<StripePaymentTransaction>()
                .AsQueryable()
                .FirstOrDefaultAsync(t => t.PaymentIntentId == failedPaymentIntent.Id);

            if (existingTransaction != null)
            {
                existingTransaction.PaymentStatus = failedPaymentIntent.Status;
                existingTransaction.Amount = failedPaymentIntent.Amount;
                existingTransaction.Currency = failedPaymentIntent.Currency;
                existingTransaction.CustomerEmail = failedPaymentIntent.ReceiptEmail;
                existingTransaction.CustomerId = failedPaymentIntent.CustomerId;
                existingTransaction.InvoiceId = failedPaymentIntent.InvoiceId;

                existingTransaction.CartId = existingTransaction?.CartId;
                existingTransaction.UserId = existingTransaction?.UserId;


                _unitOfWork.GetRepository<StripePaymentTransaction>().Update(existingTransaction);
                await _emailNotificationService.SendErrorNotificationAsync(failedPaymentIntent.ReceiptEmail, failedPaymentIntent.Id, "Payment failed");
                await _unitOfWork.SaveChangesAsync();
            }

        }
        private async Task HandleChargeSucceeded(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge == null) return;

            var existingTransaction = await _unitOfWork.GetRepository<StripePaymentTransaction>()
                .AsQueryable()
                .FirstOrDefaultAsync(t => t.PaymentIntentId == charge.PaymentIntentId);

            if (existingTransaction != null)
            {
                existingTransaction.PaymentStatus = charge.Status;
                existingTransaction.Amount = charge.Amount;
                existingTransaction.Currency = charge.Currency;
                existingTransaction.CustomerEmail = charge.BillingDetails.Email;
                existingTransaction.CustomerId = charge.CustomerId;
                existingTransaction.PaymentMethod = charge.PaymentMethod;
                existingTransaction.ChargeId = charge.Id;

                existingTransaction.UserId = existingTransaction?.UserId;
                existingTransaction.CartId = existingTransaction?.CartId;

                _unitOfWork.GetRepository<StripePaymentTransaction>().Update(existingTransaction);
                await _unitOfWork.SaveChangesAsync();
            }
            _logger.LogInformation($"Charge succeeded: {charge.Id}");
        }
        private async Task HandleChargeUpdated(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge == null) return;

            var existingTransaction = await _unitOfWork.GetRepository<StripePaymentTransaction>()
                .AsQueryable()
                .FirstOrDefaultAsync(t => t.PaymentIntentId == charge.PaymentIntentId);

            if (existingTransaction != null)
            {
                existingTransaction.PaymentStatus = charge.Status;
                existingTransaction.Amount = charge.Amount;
                existingTransaction.Currency = charge.Currency;
                existingTransaction.CustomerEmail = charge.BillingDetails.Email;
                existingTransaction.CustomerId = charge.CustomerId;
                existingTransaction.PaymentMethod = charge.PaymentMethod;
                existingTransaction.ChargeId = charge.Id;

                existingTransaction.UserId = existingTransaction?.UserId;
                existingTransaction.CartId = existingTransaction?.CartId;

                _unitOfWork.GetRepository<StripePaymentTransaction>().Update(existingTransaction);
                await _unitOfWork.SaveChangesAsync();
            }
            var paymentMethod = charge.PaymentMethod;
            var (success, message) = await _cartService.ProcessPaymentAsync
                (
                    Convert.ToInt32(existingTransaction.CartId),
                    existingTransaction.Amount / 100m, paymentMethod,
                    existingTransaction.PaymentIntentId,
                    existingTransaction.UserId
                );

            await _emailNotificationService.SendSuccessNotificationAsync
                (
                    existingTransaction.CustomerEmail,
                    existingTransaction.Amount / 100m,
                    existingTransaction.Currency, DateTime.UtcNow
                );
            _logger.LogInformation($"Charge updated: {charge.Id}");
        }
    }
}
