namespace Generic.StripeJSPaymentGateway
{
    public interface IStripeJSOptions
    {
        string StripeJSPublishableKey();
        string StripeJSApiLoginID();
        string StripeJSAccountID();
        string PayentGatewayView();
    }
}