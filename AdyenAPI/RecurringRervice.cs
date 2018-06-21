using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adyen.EcommLibrary;
using Adyen.EcommLibrary.Model.Reccuring;
using Adyen.EcommLibrary.Service.Resource.Reccuring;
using Adyen.EcommLibrary.Util;
using AbstractService = Adyen.EcommLibrary.Service.AbstractService;

namespace WebApplication1.AdyenAPI
{
    public class RecurringRervice : AbstractService
    {
        private Disable _disable;

        public RecurringRervice(Client client)
            : base(client)
        {
            this._disable = new Disable(this);
        }

        public DisableResult Disable(DisableRequest disableRequest)
        {
            try
            {
                return JsonOperation.Deserealize<DisableResult>(this._disable.Request(JsonOperation.SerializeRequest((object)disableRequest)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
