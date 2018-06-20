using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly Client _client;
        private HttpClient httpClient;

        public AdyenClient(AdyenConfiguration adyenConfiguration)
        {
            _adyenConfiguration = adyenConfiguration;
            _client = new Client(
                _adyenConfiguration.Username,
                _adyenConfiguration.Password,
                Environment.Test,
                _adyenConfiguration.MerchantAccount);

            httpClient = new HttpClient();
            var credencials = string.Format("{0}:{1}", _adyenConfiguration.Username, _adyenConfiguration.Password);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                            credencials))); ;
        }


        public void AddCreditCard()
        {

        }

        public PaymentResult CreatePayment(
            long amount,
            Shopper shopper,
            string paymentReference,
            string cardData,
            Contract recurringContract,
            string currency = "EUR",
            string selectedRecurringDetailReference = null)
        {
            var payment = new Payment(_client);
            var paymentRequest = new PaymentRequest
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
                    Contract = recurringContract
                },
                AdditionalData = new Dictionary<string, string>
                {
                    {"card.encrypted.json", cardData}
                }
            };
            if (!string.IsNullOrEmpty(selectedRecurringDetailReference))
            {
                paymentRequest.SelectedRecurringDetailReference = selectedRecurringDetailReference;
                paymentRequest.ShopperInteraction = ShopperInteraction.Ecommerce;
            }

            return payment.Authorise(paymentRequest);
        }

        public PaymentResult CreatePaymentForSubscription(long amount,
            Shopper shopper,
            string paymentReference,
            Contract recurringContract,
            string selectedRecurringDetailReference,
            string currency = "EUR")
        {
            var payment = new Payment(_client);
            var paymentRequest = new PaymentRequest
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
                    Contract = Contract.Recurring
                }
            };
            paymentRequest.SelectedRecurringDetailReference = selectedRecurringDetailReference;
            paymentRequest.ShopperInteraction = ShopperInteraction.ContAuth;

            return payment.Authorise(paymentRequest);
        }


        public RecurringDetailsResult ListRecurringDetails(string shopperReference)
        {
            var recurring = new Adyen.EcommLibrary.Service.Recurring(_client);
            var request = new RecurringDetailsRequest
            {
                ShopperReference = shopperReference,
                MerchantAccount = _adyenConfiguration.MerchantAccount
            };
            return recurring.ListRecurringDetails(request);
        }


        public ModificationResult Capture(string pspReference, string currency, long amount)
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

        public ModificationResult Refund(string pspReference, string currency, long amount)
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


    }

}
