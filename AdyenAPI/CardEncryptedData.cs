namespace WebApplication1.AdyenAPI
{
    public class CardEncryptedData
    {
        public string AdyenEncryptedData { get; set; }
    }


    public class PaymentModelView
    {
        public string AdyenEncryptedData { get; set; }
        public long Amount { get; set; }
    }
}