using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generic.StripeJSPaymentGateway.Models
{
    public class StripeJSViewModel
    {
        public string JsDomain
        {
            get
            {

                if (Development)
                {
                    return "js.stripe.com";
                }
                else
                {
                    return "js.stripe.com";
                }
            }
        }

        public bool Development { get; }

        public StripeJSViewModel(bool development)
        {
            Development = development;
        }
    }
}