using System;
using System.IO;
using System.Net;
using Adyen.EcommLibrary.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.AdyenAPI;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AdyenClient _adyenClient;

        public HomeController(AdyenConfiguration adyenConfiguration, AdyenClient adyenClient)
        {
            _adyenClient = adyenClient;
        }

        public IActionResult Index()
        {
            var shoppingCards = _adyenClient.ListRecurringDetails(Shopper.Default.Reference);
            return View("Index", shoppingCards);
        }

        #region Refund
        public IActionResult Refund()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Refund(RefundRequestDto refundRequestDto)
        {
            try
            {
                var result = _adyenClient.Refund(refundRequestDto.PspReference, "EUR", refundRequestDto.Amount);
                if (result.Response == ResponseEnum.RefundReceived)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(resp);
                return BadRequest(obj);
            }

        }

        #endregion

        #region Add Credit Card
        public IActionResult AddCreditCard()
        {
            return View("AddCreditCard", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        [HttpPost]
        public IActionResult AddCreditCard(AddCreditCardDto dto)
        {
            var shopper = Shopper.Default;
            try
            {
                var result = _adyenClient.CreatePayment(0,
                    shopper,
                    Guid.NewGuid().ToString(),
                    dto.AdyenEncryptedData,
                    dto.Contract,
                    "EUR");
                if (result.ResultCode != ResultCodeEnum.Authorised)
                {
                    return BadRequest(result);
                }
                return Redirect("/home/index");
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(resp);
                return BadRequest(obj);
            }
        }
        #endregion
        public IActionResult Pay(string recurringDetailReference)
        {
            ViewData["RecurringDetailReference"] = recurringDetailReference;
            return View("Pay");
        }

        #region Pay using saved card[onclick]

        public IActionResult PayUsingOldData(string recurringDetailReference)
        {
            ViewData["generationtime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            ViewData["RecurringDetailReference"] = recurringDetailReference;
            return View("PayUsingOldData");
        }

        [HttpPost]
        public IActionResult CreatePaymentUsingOldDataPost(string recurringDetailReference,
            PaymentModelDto data)
        {
            var shopper = Shopper.Default;
            try
            {
                var result = _adyenClient.CreatePayment(
                    data.Amount,
                    shopper,
                    Guid.NewGuid().ToString(),
                    data.AdyenEncryptedData,
                    Contract.Oneclick,
                    "EUR",
                    recurringDetailReference);
                if (result.ResultCode == ResultCodeEnum.Authorised)
                {
                    var captureResult = _adyenClient.Capture(result.PspReference, "EUR", data.Amount);
                    if (captureResult.Response == ResponseEnum.CaptureReceived)
                    {
                        return Ok(captureResult);
                    }

                    return BadRequest(captureResult);
                }
                return BadRequest(result);
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(resp);
                return Ok(obj);
            }

        }
        #endregion

        #region Pay abonnemnts action cloub be launched by user


        public IActionResult PaySubscription(string recurringDetailReference)
        {
            var shopper = Shopper.Default;
            try
            {
                const long amount = 25800;
                var result = _adyenClient.CreatePaymentForSubscription(
                    amount,
                    shopper,
                    Guid.NewGuid().ToString(),
                    Contract.Recurring,
                    recurringDetailReference, "EUR");
                if (result.ResultCode == ResultCodeEnum.Authorised)
                {
                    var captureResult = _adyenClient.Capture(result.PspReference, "EUR", amount);
                    if (captureResult.Response == ResponseEnum.CaptureReceived)
                    {
                        return Ok(captureResult);
                    }

                    return BadRequest(captureResult);
                }
                return BadRequest(result);
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(resp);
                return Ok(obj);
            }
        }
        #endregion

    }
}
