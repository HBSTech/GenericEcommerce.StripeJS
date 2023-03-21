using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.StripeJSPaymentGateway.Models
{
    public class RecaptchaRequestModel
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("remoteip")]
        public string RemoteIP { get; set; }
    }
}
