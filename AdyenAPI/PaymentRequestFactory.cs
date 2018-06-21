using System.Collections.Generic;
using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Model.Enum;

namespace WebApplication1.AdyenAPI
{
    public class PaymentRequestFactory
    {
        private static PaymentRequest Create(
            Shopper shopper, string paymentReference,
            int amount, string currency,
            string recurringDetailReference, string merchantAccount)
        {
            return new PaymentRequest
            {
                MerchantAccount = merchantAccount,
                Amount = new Amount(currency, amount),
                Reference = paymentReference,
                ShopperEmail = shopper.Email,
                ShopperReference = shopper.Reference,
                ShopperName = new Name
                {
                    FirstName = shopper.FirstName,
                    LastName = shopper.LastName
                },
                SelectedRecurringDetailReference = recurringDetailReference,
            };
        }

        public static PaymentRequest CreateForRecurringPayment(Shopper shopper,
            string paymentReference,
            int amount,
            string currency,
            string recurringDetailReference,
            string merchantAccount)
        {
            var request = Create(shopper, paymentReference, amount, currency, recurringDetailReference, merchantAccount);
            request.Recurring = new Adyen.EcommLibrary.Model.Reccuring.Recurring
            {
                Contract = Contract.Recurring
            };
            request.ShopperInteraction = ShopperInteraction.ContAuth;
            return request;
        }

        public static PaymentRequest CreateForRecurringOneclickPayment(Shopper shopper,
            string paymentReference,
            int amount,
            string currency,
            string recurringDetailReference,
            string merchantAccount,
            string cardData)
        {
            var request = Create(shopper, paymentReference, amount, currency, recurringDetailReference, merchantAccount);
            request.Recurring = new Adyen.EcommLibrary.Model.Reccuring.Recurring
            {
                Contract = Contract.Oneclick
            };
            request.AdditionalData = new Dictionary<string, string>
            {
                {"card.encrypted.json", cardData}
            };
            request.ShopperInteraction = ShopperInteraction.Ecommerce;
            return request;
        }
    }

}
