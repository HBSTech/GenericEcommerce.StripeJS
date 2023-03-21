using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.StripeJSPaymentGateway.Models
{
    public class StripeJSDataModel
    {
        [JsonPropertyName("orderGUID")]
        public Guid OrderGUID { get; set; }

        [JsonPropertyName("id")]
        public string PaymentIntentID { get; set; }

        [JsonPropertyName("token")]
        public string CaptchaToken { get; set; }
    }
}
