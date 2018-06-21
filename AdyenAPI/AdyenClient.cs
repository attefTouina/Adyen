using System;
using System.Collections.Generic;
using Adyen.EcommLibrary;
using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Model.Modification;
using Adyen.EcommLibrary.Model.Reccuring;
using Adyen.EcommLibrary.Service;
using Environment = Adyen.EcommLibrary.Model.Enum.Environment;

namespace WebApplication1.AdyenAPI
{
    public class AdyenClient : IAdyenClient
    {
        private readonly AdyenConfiguration _adyenConfiguration;
        private readonly Client _client;

        public AdyenClient(AdyenConfiguration adyenConfiguration)
        {
            _adyenConfiguration = adyenConfiguration;
            _client = new Client(
                _adyenConfiguration.Username,
                _adyenConfiguration.Password,
                Environment.Test,
                _adyenConfiguration.MerchantAccount);

        }


        public PaymentResult AddCreditCard(Shopper shopper, string cardData, string recurringContract = null)
        {
            var payment = new AuthoriseService(_client);

            var addCreditCardRequest = new AddCreditCardRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                Amount = new Amount
                {
                    Currency = "EUR",
                    Value = 0
                },
                Reference = "ADD-CREDIT-CARD-REQUEST" + Guid.NewGuid(),
                ShopperEmail = shopper.Email,
                ShopperReference = shopper.Reference,
                ShopperName = new Name
                {
                    FirstName = shopper.FirstName,
                    LastName = shopper.LastName
                },
                Recurring = new Recurring
                {
                    Contract = recurringContract ?? "ONECLICK,RECURRING"
                },
                AdditionalData = new Dictionary<string, string>
                {
                    {"card.encrypted.json", cardData}
                }
            };
            return payment.Authorise(addCreditCardRequest);
        }

        public PaymentResult AuthoriseRecurringOnClick(int amount,
            string currency,
            Shopper shopper,
            string paymentReference,
            string recurringDetailReference,
            string cardData)
        {
            var payment = new Payment(_client);
            
            var paymentRequest = PaymentRequestFactory.CreateForRecurringOneclickPayment(
                shopper, paymentReference,
                amount, currency, recurringDetailReference,
                _adyenConfiguration.MerchantAccount, cardData
            );

            return payment.Authorise(paymentRequest);
        }

        public PaymentResult AuthoriseRecurring(int amount,
            string currency,
            Shopper shopper,
            string paymentReference,
            string recurringDetailReference)
        {
            var payment = new Payment(_client);
            var paymentRequest = PaymentRequestFactory.CreateForRecurringPayment(
                shopper, 
                paymentReference, 
                amount, 
                currency,
                recurringDetailReference, 
                _adyenConfiguration.MerchantAccount
            );
            return payment.Authorise(paymentRequest);
        }


        public RecurringDetailsResult GetRecurringDetails(string shopperReference)
        {
            var recurring = new Adyen.EcommLibrary.Service.Recurring(_client);
            var request = new RecurringDetailsRequest
            {
                ShopperReference = shopperReference,
                MerchantAccount = _adyenConfiguration.MerchantAccount
            };
            return recurring.ListRecurringDetails(request);
        }

        public ModificationResult Capture(string pspReference, string currency, int amount)
        {
            var modification = new Modification(_client);
            var captureRequest = new CaptureRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                ModificationAmount = new Amount(currency, amount),
                OriginalReference = pspReference
            };
            return modification.Capture(captureRequest);
        }

        public ModificationResult Refund(string pspReference, string currency, int amount)
        {
            var modification = new Modification(_client);

            var captureRequest = new RefundRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                ModificationAmount = new Amount(currency, amount),
                OriginalReference = pspReference
            };
            return modification.Refund(captureRequest);
        }

        public DisableResult Disable(string recurringDetailReference, string shopperReference)
        {
            var recurring = new RecurringRervice(_client);
            var request = new DisableRequest
            {
                MerchantAccount = _adyenConfiguration.MerchantAccount,
                RecurringDetailReference = recurringDetailReference,
                ShopperReference = shopperReference
            };
            return recurring.Disable(request);
        }
    }

}
