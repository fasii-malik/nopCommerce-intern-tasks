using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Stripe.Models;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Stripe.Fields.SecretKey")]
    public string SecretKey { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Payments.Stripe.Fields.PublishableKey")]
    public string PublishableKey { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Payments.Stripe.Fields.UseTestMode")]
    public bool UseTestMode { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Stripe.Fields.AdditionalFee")]
    public decimal AdditionalFee { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Stripe.Fields.AdditionalFeePercentage")]
    public bool AdditionalFeePercentage { get; set; }
}