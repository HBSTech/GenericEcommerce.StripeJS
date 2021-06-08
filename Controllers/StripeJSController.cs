using Generic.StripeJSPaymentGateway.Models;
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

namespace Generic.StripeJSPaymentGateway.Controllers
{
    public class StripeJSController : Controller
    {
        public IOrderInfoProvider OrderInfoProvider { get; }
        public IStripeJSOptions StripeJSOptions { get; }

        public StripeJSController(IOrderInfoProvider orderInfoProvider, IStripeJSOptions stripeJSOptions)
        {
            OrderInfoProvider = orderInfoProvider;
            StripeJSOptions = stripeJSOptions;
        }

        public async Task<IActionResult> GetAuthorization()
        {
            var model = new AuthorizationDataModel
            {
                PublishableKey = StripeJSOptions.StripeJSPublishableKey()
            };
            return new JsonResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPaymentIntent([FromBody] StripeJSDataModel model)
        {
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

            var requestOptions = new RequestOptions
            {
                ApiKey = StripeJSOptions.StripeJSApiLoginID(),
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
                if (e.StripeError.Type == "card_error")
                {
                    return new JsonResult(new { Message = "Failed Transaction. Rate Limit reached.  Please wait 60 seconds and try again" });
                }
                else
                {
                    // Creates a payment result object that will be viewable in Xperience
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = $"Failed Transaction. {e.StripeError.Code}: {e.StripeError.Message}.",
                        PaymentIsCompleted = false,
                        PaymentIsFailed = true,
                        PaymentTransactionID = e.StripeError.StripeResponse.RequestId,
                        PaymentStatusValue = $"Response Code: {e.StripeError.Code}, Message: {e.StripeError.Message},  Description: { e.StripeError.ErrorDescription}",
                        PaymentMethodName = "StripeJS"
                    };

                    // Saves the payment result to the database
                    order.UpdateOrderStatus(result);

                    return new JsonResult(new { error = $"Failed Transaction. {e.StripeError.Code}: {e.StripeError.Message}.  Please contact us and refrence {order.OrderID}." });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment([FromBody] StripeJSDataModel model)
        {
            var requestOptions = new RequestOptions
            {
                ApiKey = StripeJSOptions.StripeJSApiLoginID(),
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
                    PaymentDescription = $"Failed Transaction. {e.StripeError.Code}: {e.StripeError.Message}.",
                    PaymentIsCompleted = false,
                    PaymentIsFailed = true,
                    PaymentTransactionID = e.StripeError.StripeResponse.RequestId,
                    PaymentStatusValue = $"Response Code: {e.StripeError.Code}, Message: {e.StripeError.Message},  Description: { e.StripeError.ErrorDescription}",
                    PaymentMethodName = "StripeJS"
                };

                // Saves the payment result to the database
                order.UpdateOrderStatus(result);

                return new JsonResult(new { Message = $"Failed Transaction. {e.StripeError.Code}: {e.StripeError.Message}.  Please contact us and refrence {order.OrderID}." });
            }
            return new JsonResult(new { Message = "Transaction Failed." });
        }
    }
}
