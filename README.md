# GenericEcommerce.StripeJS
Stripe JS Payment gateway for [Generic Ecommerce module](https://www.nuget.org/packages/HBS.Kentico.Ecommerce/)

To use make sure your application is already using the [Generic.Ecommerce solution](https://www.nuget.org/packages/HBS.Kentico.Ecommerce/).

## Installation
1. Install the [HBS.Kentico.Ecommerce.StripeJS](https://www.nuget.org/packages/HBS.Kentico.Ecommerce.StripeJS/) nuget package onto your web.config
1. In your `Startups.cs`, register on the services using this command:
`services.RegisterStripeJS(new StripeJSConfiguration("7ae4gSdBP6pbLVkAXeRLSmFyuWxR9Ku23j7a8wUQv37RYsH8B7w36573W5sZb8vG", "2LE2uEtp4rq", "74bg98n65MEh4S8x"));`
1. In Kentico Xperience's Admin UI, Create a Payment Option with the Code Name of `StripeJS`

### Customize View
You can optionally provide your own stripejs view by creating a view at the path matching the string `StripeJSConfiguration.PayentGatewayView` property.  This will overwrite the default.
