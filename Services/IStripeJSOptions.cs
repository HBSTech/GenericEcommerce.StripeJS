namespace Generic.StripeJSPaymentGateway
{
    public interface IStripeJSOptions
    {
        string StripeJSPublishableKey();
        string StripeJSSecretKey();
        string StripeJSAccountID();
        string PayentGatewayView();
        string ReCaptchaPublicKey();
        string ReCaptchaPrivateKey();
    }
}