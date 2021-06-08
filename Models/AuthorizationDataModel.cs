using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.StripeJSPaymentGateway.Models
{
    public class AuthorizationDataModel
    {
        [JsonPropertyName("publishableKey")]
        public string PublishableKey { get; set; }
        [JsonPropertyName("apiLoginID")]
        public string ApiLoginID { get; set; }
    }
}
