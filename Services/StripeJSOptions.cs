using System;
using System.Collections.Generic;
using System.Text;

namespace Generic.StripeJSPaymentGateway
{
    public class StripeJSOptions : IStripeJSOptions
    {
        public StripeJSOptions(StripeJSConfiguration stripeJSConfiguration)
        {
            StripeJSConfiguration = stripeJSConfiguration;
        }

        public StripeJSConfiguration StripeJSConfiguration { get; }

        public string StripeJSSecretKey()
        {
            return StripeJSConfiguration.StripeJSSecretKey;
        }

        public string StripeJSAccountID()
        {
            return StripeJSConfiguration.StripeJSAccountID;
        }

        public string StripeJSPublishableKey()
        {
            return StripeJSConfiguration.StripeJSPublishableKey;
        }

        public string PayentGatewayView()
        {
            return StripeJSConfiguration.PayentGatewayView;
        }
    }
}
