using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Adyen.EcommLibrary;
using Adyen.EcommLibrary.Model;
using Adyen.EcommLibrary.Model.Enum;
using Adyen.EcommLibrary.Model.Reccuring;
using Adyen.EcommLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.AdyenAPI;
using Environment = Adyen.EcommLibrary.Model.Enum.Environment;
using Recurring = Adyen.EcommLibrary.Service.Recurring;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AdyenConfiguration _adyenConfiguration;
        private readonly AdyenClient _adyenClient;

        public HomeController(AdyenConfiguration adyenConfiguration, AdyenClient adyenClient)
        {
            _adyenConfiguration = adyenConfiguration;
            this._adyenClient = adyenClient;
        }

        public IActionResult Index()
        {
            var shoppingCards = _adyenClient.ListRecurringDetails(Shopper.Default.Reference);
            return View("Index", shoppingCards);
        }


        public IActionResult AddCreditCard()
        {
            return View("AddCreditCard", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        [HttpPost]
        public IActionResult AddCreditCard(CreditCardData data)
        {
            var shopper = Shopper.Default;
            try
            {
                var result = _adyenClient.CreatePayment(0,
                    shopper,
                    Guid.NewGuid().ToString(),
                    data.AdyenEncryptedData,
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



        //first time 
        public IActionResult CreatePayment()
        {
            return View("CreatePayment", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        [HttpPost]
        public IActionResult CreatePayment(CardEncryptedData data)
        {
            var shopper = new Shopper
            {
                Email = "attef@alphorm.com",
                FirstName = "Attef",
                LastName = "Touina",
                Reference = "AttefRef"
            };

            var result = _adyenClient.CreatePayment(1500,
                shopper,
                Guid.NewGuid().ToString(),
                data.AdyenEncryptedData,
                "EUR");

            _adyenClient.Capture(result.PspReference, "EUR", 1000);
            return Ok(result);
        }

        public IActionResult Details(string shopperRef)
        {
            return Ok(_adyenClient.ListRecurringDetails(shopperRef));
        }

        public IActionResult CreatePaymentUsingOldData(string recurringDetailReference)
        {
            ViewData["generationtime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            ViewData["RecurringDetailReference"] = recurringDetailReference;
            return View("CreatePaymentUsingOldData");
        }

        [HttpPost]
        public IActionResult CreatePaymentUsingOldDataPost(string recurringDetailReference,
            PaymentModelView data)
        {
            var shopper = Shopper.Default;
            try
            {
                var result = _adyenClient.CreatePayment(
                    data.Amount,
                    shopper,
                    Guid.NewGuid().ToString(),
                    data.AdyenEncryptedData,
                    "EUR",
                    recurringDetailReference);
                if(result.ResultCode == ResultCodeEnum.Authorised)
                {
                    var captureResult = _adyenClient.Capture(result.PspReference, "EUR", data.Amount);
                    if(captureResult.Response == ResponseEnum.CaptureReceived)
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
    }
}
