using System.Collections.Generic;
using System.Runtime.Serialization;
using Adyen.EcommLibrary.Model;

namespace WebApplication1.AdyenAPI
{
    [DataContract]
    public class AddCreditCardRequest
    {
        [DataMember(EmitDefaultValue = false, Name = "merchantAccount")]
        public string MerchantAccount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "reference")]
        public string Reference { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "amount")]
        public Amount Amount = new Amount("EUR", 0);

        [DataMember(EmitDefaultValue = false, Name = "shopperEmail")]
        public string ShopperEmail { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "shopperReference")]
        public string ShopperReference { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "shopperName")]
        public Name ShopperName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "additionalData")]
        public Dictionary<string, string> AdditionalData { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "recurring")]
        public Recurring Recurring { get; set; }
    }

    [DataContract]
    public class Recurring
    {
        [DataMember(EmitDefaultValue = false, Name = "contract")]
        public string Contract { get; set; }
    }
}
