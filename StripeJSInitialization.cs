using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System;
using System.Collections.Generic;
using System.Reflection;
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
                Name = Assembly.GetEntryAssembly().GetName().Name,
                Url = "https://github.com/HBSTech/GenericEcommerce.StripeJS",
                Version = Assembly.GetEntryAssembly().GetName().Version.ToString()
            };
            StripeConfiguration.MaxNetworkRetries = 5;
            StripeConfiguration.ApiKey = options.StripeJSSecretKey;
            return services;
        }
    }
}
