using System;
using Adyen.EcommLibrary;
using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Service.Resource.Payment;
using Adyen.EcommLibrary.Util;
using Newtonsoft.Json;

namespace WebApplication1.AdyenAPI
{
    public class AuthoriseService : Adyen.EcommLibrary.Service.AbstractService
    {
            private readonly Authorise _authorise;

            public AuthoriseService(Client client)
                : base(client)
            {
                this._authorise = new Authorise(this);
            }

            public PaymentResult Authorise(AddCreditCardRequest paymentRequest)
            {
                try
                {
                    return JsonConvert.DeserializeObject<PaymentResult>(this._authorise.Request(JsonOperation.SerializeRequest((object)paymentRequest)));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
