using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Stripe;

public class StripePaymentSettings : ISettings
{
    /// <summary>Stripe Secret Key (sk_live_... or sk_test_...)</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Stripe Publishable Key (pk_live_... or pk_test_...)</summary>
    public string PublishableKey { get; set; } = string.Empty;

    /// <summary>Use test mode</summary>
    public bool UseTestMode { get; set; } = true;

    /// <summary>Additional transaction fee</summary>
    public decimal AdditionalFee { get; set; }

    public bool AdditionalFeePercentage { get; set; }
}