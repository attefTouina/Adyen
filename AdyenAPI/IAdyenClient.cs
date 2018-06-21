using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Model.Modification;
using Adyen.EcommLibrary.Model.Reccuring;

namespace WebApplication1.AdyenAPI
{
    public interface IAdyenClient
    {
        PaymentResult AddCreditCard(Shopper shopper, string cardData, string recurringContract = null);
        PaymentResult AuthoriseRecurring(int amount, string currency, Shopper shopper, string paymentReference, string recurringDetailReference);
        PaymentResult AuthoriseRecurringOnClick(int amount, string currency, Shopper shopper, string paymentReference, string recurringDetailReference, string cardData);
        ModificationResult Capture(string pspReference, string currency, int amount);
        RecurringDetailsResult GetRecurringDetails(string shopperReference);
        ModificationResult Refund(string pspReference, string currency, int amount);
        DisableResult Disable(string recurringDetailReference, string shopperReference);
    }
}