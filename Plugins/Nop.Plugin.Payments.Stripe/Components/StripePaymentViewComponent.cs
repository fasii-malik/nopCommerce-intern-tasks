// Components/StripePaymentViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Stripe.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Stripe.Components;

public class StripePaymentViewComponent : NopViewComponent
{
    private readonly StripePaymentSettings _settings;

    public StripePaymentViewComponent(StripePaymentSettings settings)
    {
        _settings = settings;
    }

    public IViewComponentResult Invoke()
    {
        var model = new PaymentInfoModel
        {
            PublishableKey = _settings.PublishableKey
        };

        return View("~/Plugins/Payments.Stripe/Views/PaymentInfo.cshtml", model);
    }
}