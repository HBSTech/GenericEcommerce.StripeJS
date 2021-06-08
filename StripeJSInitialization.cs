using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generic.StripeJSPaymentGateway
{
    public static class StripeJSInitialization
    {
        public static IServiceCollection RegisterStripeJS(this IServiceCollection services, StripeJSConfiguration options)
        {
            services.AddSingleton<IStripeJSOptions>(new StripeJSOptions(options));
            StripeConfiguration.AppInfo = new AppInfo
            {
                Name = "HBS.Kentico.Ecommerce.StripePaymentGateway",
                Url = "https://github.com/HBSTech/GenericEcommerce.StripeJS",
                Version = "1.0.0"
            };
            StripeConfiguration.MaxNetworkRetries = 5;
            return services;
        }
    }
}
