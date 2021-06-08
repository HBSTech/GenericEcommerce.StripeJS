using Generic.StripeJSPaymentGateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Generic.StripeJSPaymentGateway.Components.Accept
{
    [ViewComponent(Name = "Generic.Ecom.StripeJS")]

    public class StripeJSViewComponent : ViewComponent
    {
        public StripeJSViewComponent(IHostEnvironment env, IStripeJSOptions stripeJSOptions)
        {
            Env = env;
            StripeJSOptions = stripeJSOptions;
        }

        private IHostEnvironment Env { get; }
        public IStripeJSOptions StripeJSOptions { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(StripeJSOptions.PayentGatewayView(), new StripeJSViewModel(Env.IsDevelopment()));
        }
    }
}
