namespace Nop.Plugin.Payments.Stripe;

public static class StripeDefaults
{
    public const string SystemName = "Payments.Stripe";
    public const string ConfigurationRouteName = "Plugin.Payments.Stripe.Configure";
    public const string PaymentInfoViewComponentName = "StripePaymentInfo";

    // Stripe API endpoints
    public const string StripeApiBase = "https://api.stripe.com/v1";
    public const string PaymentIntentsEndpoint = "/payment_intents";
    public const string PaymentIntentsConfirmSuffix = "/confirm";
}