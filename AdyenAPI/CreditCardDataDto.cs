using Adyen.EcommLibrary.Model.Enum;

namespace WebApplication1.AdyenAPI
{
    public class AddCreditCardDto
    {
        public string AdyenEncryptedData { get; set; }

        public Contract Contract { get; set; }
    }
}