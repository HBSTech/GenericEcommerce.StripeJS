﻿using Generic.StripeJSPaymentGateway.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CMS.DataEngine;
using CMS.SiteProvider;
using System.Net;
using CMS.Ecommerce;
using System.Linq;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Core;
using Stripe;
using System.Text.Json;
using System.IO;

namespace Generic.StripeJSPaymentGateway.Controllers
{
    public class StripeJSController : Controller
    {
        public IOrderInfoProvider OrderInfoProvider { get; }
        public IStripeJSOptions StripeJSOptions { get; }
        public IEventLogService EventLogService { get; }

        public StripeJSController(IOrderInfoProvider orderInfoProvider, IStripeJSOptions stripeJSOptions, IEventLogService eventLogService)
        {
            OrderInfoProvider = orderInfoProvider;
            StripeJSOptions = stripeJSOptions;
            EventLogService = eventLogService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAuthorization()
        {
            var model = new AuthorizationDataModel
            {
                PublishableKey = EncodeBase64(EncodeBase64(StripeJSOptions.StripeJSPublishableKey() + StripeJSOptions.GetObscurificationKey())) 
            };
            return new JsonResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPaymentIntent([FromBody] StripeJSDataModel model)
        {
            using (var client = new WebClient())
            {
                client.BaseAddress = "https://www.google.com";
                string responseText;
                try
                {
                    var response = client.UploadString($"recaptcha/api/siteverify?secret={StripeJSOptions.ReCaptchaPrivateKey()}&response={model.CaptchaToken}", "");
                    var responseModel = JsonSerializer.Deserialize<RecaptchaResponseModel>(response);
                    if(!responseModel.Success)
                    {
                        return new JsonResult(new { error = string.Join(',',responseModel.Errors) });
                    }
                }
                catch (WebException ex)
                {
                    var responseStream = ex.Response?.GetResponseStream();

                    if (responseStream != null)
                    {
                        using var reader = new StreamReader(responseStream);
                        responseText = reader.ReadToEnd();
                        return new JsonResult(new { error = responseText });
                    }
                }
                catch(Exception ex)
                {
                    return new JsonResult(new { error = ex.InnerException });

                }
            }

            var order = (await OrderInfoProvider.Get().WhereEquals(nameof(OrderInfo.OrderGUID), model.OrderGUID).TopN(1).GetEnumerableTypedResultAsync()).FirstOrDefault();

            var mainCurrency = Service.Resolve<ISiteMainCurrencySource>().GetSiteMainCurrency(order.OrderSiteID);

            var options = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt32(order.OrderGrandTotalInMainCurrency * Convert.ToInt32(Math.Pow(10, mainCurrency.CurrencyRoundTo))),
                Currency = mainCurrency.CurrencyCode.ToLowerInvariant(),
                Description = $"Online kentico charge for order {order.OrderID}",
                PaymentMethodTypes = new List<string>
                {
                    "card"
                }
            };
            (options.Metadata ??= new Dictionary<string, string>())
                .Add("OrderID", order.OrderID.ToString());

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guid.NewGuid().ToString(),
                StripeAccount = StripeJSOptions.StripeJSAccountID()
            };
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options, requestOptions);
                return new JsonResult(new { id = paymentIntent.Id, clientSecret = paymentIntent.ClientSecret });
            }
            catch (StripeException e)
            {
                if (e?.StripeError?.Type == "rate_limit")
                {
                    return new JsonResult(new { error = "Failed Transaction. Rate Limit reached.  Please wait 60 seconds and try again" });
                }
                else
                {
                    // Creates a payment result object that will be viewable in Xperience
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = $"Failed Transaction. {e?.StripeError?.Code}: {e?.StripeError?.Message}.",
                        PaymentIsCompleted = false,
                        PaymentIsFailed = true,
                        PaymentTransactionID = e?.StripeError?.StripeResponse?.RequestId,
                        PaymentStatusValue = $"Response Code: {e?.StripeError?.Code}, Message: {e?.StripeError?.Message},  Description: { e?.StripeError?.ErrorDescription}",
                        PaymentMethodName = "StripeJS"
                    };

                    // Saves the payment result to the database
                    order.UpdateOrderStatus(result);

                    return new JsonResult(new { error = $"Failed Transaction. {e?.StripeError?.Code}: {e?.StripeError?.Message}.  Please contact us and refrence {order?.OrderID}." });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment([FromBody] StripeJSDataModel model)
        {
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guid.NewGuid().ToString(),
                StripeAccount = StripeJSOptions.StripeJSAccountID() 
            };

            var order = (await OrderInfoProvider.Get().WhereEquals(nameof(OrderInfo.OrderGUID), model.OrderGUID).TopN(1).GetEnumerableTypedResultAsync()).FirstOrDefault();

            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(model.PaymentIntentID, requestOptions: requestOptions);

                // validate response
                if (paymentIntent.StripeResponse.StatusCode == HttpStatusCode.OK)
                {

                    // Creates a payment result object that will be viewable in Xperience
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = "Transaction with Transaction ID: " + paymentIntent.StripeResponse.RequestId,
                        PaymentIsCompleted = paymentIntent.Status == "succeeded",
                        PaymentIsFailed = paymentIntent.Status == "requires_payment_method",
                        PaymentTransactionID = paymentIntent.StripeResponse.RequestId,
                        PaymentStatusValue = $"Response Code: {paymentIntent.StripeResponse.StatusCode},  Description: { paymentIntent.StripeResponse.Content}",
                        PaymentMethodName = "StripeJS"
                    };

                    // Saves the payment result to the database
                    order.UpdateOrderStatus(result);

                    return new JsonResult(new { PaymentSuccessful = paymentIntent.Status == "succeeded" });
                }
            }
            catch (StripeException e)
            {
                // Creates a payment result object that will be viewable in Xperience
                PaymentResultInfo result = new PaymentResultInfo
                {
                    PaymentDate = DateTime.Now,
                    PaymentDescription = $"Failed Transaction. {e?.StripeError?.Code}: {e?.StripeError?.Message}.",
                    PaymentIsCompleted = false,
                    PaymentIsFailed = true,
                    PaymentTransactionID = e?.StripeError?.StripeResponse?.RequestId,
                    PaymentStatusValue = $"Response Code: {e?.StripeError?.Code}, Message: {e?.StripeError?.Message},  Description: { e?.StripeError?.ErrorDescription}",
                    PaymentMethodName = "StripeJS"
                };

                // Saves the payment result to the database
                order.UpdateOrderStatus(result);

                return new JsonResult(new { Message = $"Failed Transaction. {e?.StripeError?.Code}: {e?.StripeError?.Message}.  Please contact us and refrence {order?.OrderID}." });
            }
            return new JsonResult(new { Message = "Transaction Failed." });
        }

        private string EncodeBase64(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
    }
}
