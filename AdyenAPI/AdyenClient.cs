using System.Collections.Generic;
using Adyen.EcommLibrary;
using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Model.Enum;
using Adyen.EcommLibrary.Model.Modification;
using Adyen.EcommLibrary.Model.Reccuring;
using Adyen.EcommLibrary.Service;
using Environment = Adyen.EcommLibrary.Model.Enum.Environment;

namespace WebApplication1.AdyenAPI
{
    public class AdyenClient
    {
        private readonly AdyenConfiguration _adyenConfiguration;
        private Client client;

        public AdyenClient(AdyenConfiguration adyenConfiguration)
        {
            _adyenConfiguration = adyenConfiguration;
            client = new Client(
                _adyenConfiguration.Username,
                _adyenConfiguration.Password,
                Environment.Test,
                _adyenConfiguration.MerchantAccount);
        }

        public PaymentResult CreatePayment(
            long amount,
            Shopper shopper,
            string paymentReference,
            string cardData,
            string currency = "EUR",
            string selectedRecurringDetailReference = null)
        {
            var payment = new Payment(client);
            var paymentRequest = CreatePaymentRequest(currency, amount, paymentReference,
                shopper, cardData);
            if (!string.IsNullOrEmpty(selectedRecurringDetailReference))
            {
                paymentRequest.SelectedRecurringDetailReference = selectedRecurringDetailReference;
                paymentRequest.ShopperInteraction = ShopperInteraction.Ecommerce;
            }

            return payment.Authorise(paymentRequest);
        }

        private PaymentRequest CreatePaymentRequest(string currency,
            long amount, string paymentReference, Shopper shopper, string cardData)
        {
            return new PaymentRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                Amount = new Amount(currency, amount),
                Reference = paymentReference,
                ShopperEmail = shopper.Email,
                ShopperReference = shopper.Reference,
                ShopperName = new Name
                {
                    FirstName = shopper.FirstName,
                    LastName = shopper.LastName
                },
                Recurring = new Adyen.EcommLibrary.Model.Reccuring.Recurring
                {
                    Contract = Contract.Oneclick
                },
                AdditionalData = new Dictionary<string, string>
                {
                    {"card.encrypted.json", cardData}
                }
            };
        }


        public RecurringDetailsResult ListRecurringDetails(string shopperReference)
        {
            var recurring = new Adyen.EcommLibrary.Service.Recurring(client);
            var request = new RecurringDetailsRequest
            {
                ShopperReference = shopperReference,
                MerchantAccount = _adyenConfiguration.MerchantAccount
            };
            return recurring.ListRecurringDetails(request);
        }


        public ModificationResult Capture(string pspReference, string currency, long amount)
        {
            var modification = new Modification(client);

            var captureRequest = new CaptureRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                ModificationAmount = new Amount(currency, amount),
                OriginalReference= pspReference
            };
           return modification.Capture(captureRequest);
        }

        public ModificationResult Refund(string pspReference, string currency, long amount)
        {
            var modification = new Modification(client);

            var captureRequest = new RefundRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                ModificationAmount = new Amount(currency, amount),
                OriginalReference = pspReference
            };
            return modification.Refund(captureRequest);
        }


    }

}
