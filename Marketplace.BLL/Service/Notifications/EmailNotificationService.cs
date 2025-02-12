using Marketplace.BLL.Contracts.Notifications;
using Marketplace.BLL.Settings.Notifications;
using Marketplace.Domain.PaymentTransactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Notifications
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly SmtpSetting _smtpSettings;
        private readonly SmtpClient _smtpClient;

        public EmailNotificationService(ILogger<EmailNotificationService> logger, IOptions<SmtpSetting> smtpSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;

            _smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                EnableSsl = true
            };
        }
        public async Task SendErrorNotificationAsync(string toEmail, string paymentId, string errorMessage)
        {
            string subject = "Payment Error";
            string body = $"There was an error with your payment ID {paymentId}. Error: {errorMessage}";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendInsufficientFundsNotificationAsync(string toEmail, string paymentId)
        {
            string subject = "Payment Failed - Insufficient Funds";
            string body = $"Your payment with ID {paymentId} failed due to insufficient funds.";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendRefundNotificationEmailForStripe(StripePaymentTransaction transaction)
        {
            try
            {
                var subject = $"Refund Processed for Payment #{transaction.PaymentIntentId}";
                var body = $@"
            <h3>Refund Processed Successfully</h3>
            <p>We have successfully processed your refund for payment #{transaction.PaymentIntentId}.</p>
            <p><strong>Payment Details:</strong></p>
            <ul>
                <li><strong>Amount:</strong> {(transaction.Amount / 100.0):F2} {transaction.Currency}</li>
                <li><strong>Transaction ID:</strong> {transaction.PaymentIntentId}</li>
                <li><strong>Status:</strong> Refunded</li>
            </ul>
            <p>We appreciate your business and hope to see you again soon!</p>
            <p>Best regards, <br/> Your Company</p>
        ";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(transaction.CustomerEmail ?? throw new Exception("Customer email is missing"));

                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Refund notification email sent to {transaction.CustomerEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send refund notification email for payment {transaction.PaymentIntentId}");
                throw;
            }
        }

        public async Task SendSuccessNotificationAsync(string email, decimal amount, string currency, DateTime paymentTime)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.SenderName),
                    Subject = "Payment Successful",
                    Body = $@"
                <h3>Thank you for your payment!</h3>
                <p>Your payment has been successfully processed.</p>
                <p><strong>Payment Details:</strong></p>
                <ul>
                    <li><strong>Amount:</strong> {amount} {currency}</li>
                    <li><strong>Date and Time:</strong> {paymentTime.ToString("f")}</li>
                </ul>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await _smtpClient.SendMailAsync(mailMessage);  // Используем _smtpClient, созданный в конструкторе
                _logger.LogInformation($"Email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                throw;
            }
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage(_smtpSettings.FromEmail, toEmail, subject, body);
                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {toEmail} with subject: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
            }
        }
    }
}
