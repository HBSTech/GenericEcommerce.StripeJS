using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.StripeJSPaymentGateway.Models
{
    public class RecaptchaResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTS { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public IEnumerable<string> Errors { get;set; }

        [JsonPropertyName("score")]
        public double Score {get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
