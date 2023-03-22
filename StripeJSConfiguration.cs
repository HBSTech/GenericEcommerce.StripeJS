namespace Generic.StripeJSPaymentGateway
{
    public class StripeJSConfiguration
    {
        public StripeJSConfiguration(string stripeJSPublishableKey, string stripeJSSecretKey, string stripeJSAccountID, string stripeJSObscurificationKey)
        {
            StripeJSPublishableKey = stripeJSPublishableKey;
            StripeJSSecretKey = stripeJSSecretKey;
            StripeJSAccountID = stripeJSAccountID;
            StripeJSObscurificationKey = stripeJSObscurificationKey;
        }

        /// <summary>
        /// Log into Authorize.net -> Account -> "Manage Public Client Key" and create new key
        /// </summary>
        public string StripeJSPublishableKey { get; set; }

        /// <summary>
        /// APi LoginID: Log into Authorize.net -> Account -> API Credentials & Keys -> "API Login ID" is on the page.
        /// </summary>
        public string StripeJSSecretKey { get; set; }

        /// <summary>
        /// APi LoginID: Log into Authorize.net -> Account -> API Credentials & Keys -> "API Login ID" is on the page.
        /// </summary>
        public string StripeJSObscurificationKey { get; set; }

        /// <summary>
        /// Log into Authorize.net -> Account -> API Credentials & Keys -> New Transaction Key (WARNING: THIS DISABLES CURRENT TRANSACTION KEYS)
        /// </summary>
        public string StripeJSAccountID { get;set; }

        public string PayentGatewayView { get; set; } = "~/Components/StripeJS/StripeJS.cshtml";
    }
}