using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Stripe.Models;

public record PaymentInfoModel : BaseNopModel
{
    /// <summary>Stripe.js publishable key — injected into the view</summary>
    public string PublishableKey { get; set; } = string.Empty;
}