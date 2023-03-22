using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;
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

        public string ReCaptchaPublicKey()
        {
            return SettingsKeyInfoProvider.GetValue(new SettingsKeyName("CMSReCaptchaPublicKey", new SiteInfoIdentifier(SiteContext.CurrentSiteID)));
        }

        public string ReCaptchaPrivateKey()
        {
            return SettingsKeyInfoProvider.GetValue(new SettingsKeyName("CMSReCaptchaPrivateKey", new SiteInfoIdentifier(SiteContext.CurrentSiteID)));
        }

        public string GetObscurificationKey()
    {
            return StripeJSConfiguration.StripeJSObscurificationKey;
    }
    }
}
