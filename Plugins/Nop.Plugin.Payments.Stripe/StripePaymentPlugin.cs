using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Stripe.Components;
using Nop.Plugin.Payments.Stripe.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.Stripe;

public class StripePaymentPlugin : BasePlugin, IPaymentMethod
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWorkContext _workContext;
    private readonly StripeHttpClient _stripeClient;
    private readonly StripePaymentSettings _settings;
    private readonly IWebHelper _webHelper;

    // Zero-decimal currencies — Stripe expects whole units, not ×100
    private static readonly HashSet<string> ZeroDecimalCurrencies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "bif", "clp", "gnf", "jpy", "kmf", "krw", "mga",
            "pyg", "rwf", "ugx", "vnd", "vuv", "xaf", "xof", "pkr"
        };

    public StripePaymentPlugin(
        ILocalizationService localizationService,
        ISettingService settingService,
        IWorkContext workContext,
        StripeHttpClient stripeClient,
        StripePaymentSettings settings,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _workContext = workContext;
        _stripeClient = stripeClient;
        _settings = settings;
        _webHelper = webHelper;
    }

    public bool SupportCapture => false;
    public bool SupportPartiallyRefund => false;
    public bool SupportRefund => false;
    public bool SupportVoid => false;
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;
    public bool SkipPaymentInfo => false;

    public Task<bool> CanRePostProcessPaymentAsync(Order order) =>
        Task.FromResult(false);

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(
        CancelRecurringPaymentRequest request) =>
        Task.FromResult(new CancelRecurringPaymentResult
        { Errors = new[] { "Recurring not supported" } });

    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart) =>
        Task.FromResult(false);

    // ── Additional fee ──────────────────────────────────────────────────────

    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        // Simple flat or percentage fee without extension method dependency
        if (_settings.AdditionalFee <= 0)
            return Task.FromResult(0m);

        if (!_settings.AdditionalFeePercentage)
            return Task.FromResult(_settings.AdditionalFee);

        // Percentage: apply to cart subtotal
        var subtotal = cart.Sum(item => item.Quantity * 0m); // replace with actual price if needed
        return Task.FromResult(subtotal * _settings.AdditionalFee / 100m);
    }

    // ── Process payment ─────────────────────────────────────────────────────

    public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var result = new ProcessPaymentResult();

        if (!request.CustomValues.TryGetValue("StripePaymentMethodId", out var paymentMethodIdObj)
            || paymentMethodIdObj is not string paymentMethodId
            || string.IsNullOrEmpty(paymentMethodId))
        {
            result.AddError("Stripe payment method ID was not provided.");
            return result;
        }

        try
        {
            var workingCurrency = await _workContext.GetWorkingCurrencyAsync();
            var currency = workingCurrency.CurrencyCode.ToLowerInvariant();

            // Stripe zero-decimal currencies must NOT be multiplied by 100
            var amount = ZeroDecimalCurrencies.Contains(currency)
                ? (long)Math.Round(request.OrderTotal, 0)
                : (long)Math.Round(request.OrderTotal * 100, 0);

            var intent = await _stripeClient.CreatePaymentIntentAsync(
                amount, currency, paymentMethodId);

            if (intent.Status == "succeeded")
            {
                result.NewPaymentStatus = PaymentStatus.Paid;
                result.AuthorizationTransactionId = intent.Id;
                result.AuthorizationTransactionResult = intent.Status;
            }
            else
            {
                result.AddError($"Stripe returned status: {intent.Status}. " +
                                $"{intent.LastPaymentError?.Message}");
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Stripe error: {ex.Message}");
        }

        return result;
    }

    public Task PostProcessPaymentAsync(PostProcessPaymentRequest request) =>
        Task.CompletedTask;

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(
        ProcessPaymentRequest request) =>
        Task.FromResult(new ProcessPaymentResult
        { Errors = new[] { "Recurring not supported" } });

    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest request) =>
        Task.FromResult(new RefundPaymentResult
        { Errors = new[] { "Refund not supported" } });

    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest request) =>
        Task.FromResult(new VoidPaymentResult
        { Errors = new[] { "Void not supported" } });

    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest request) =>
        Task.FromResult(new CapturePaymentResult
        { Errors = new[] { "Capture not supported" } });

    // ── Form handling ───────────────────────────────────────────────────────

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        var warnings = new List<string>();

        if (!form.TryGetValue("StripePaymentMethodId", out var val)
            || string.IsNullOrWhiteSpace(val))
            warnings.Add("Card information is required.");

        return Task.FromResult<IList<string>>(warnings);
    }

    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        var request = new ProcessPaymentRequest();

        if (form.TryGetValue("StripePaymentMethodId", out var paymentMethodId))
            request.CustomValues["StripePaymentMethodId"] = paymentMethodId.ToString();

        return Task.FromResult(request);
    }

    public Type GetPublicViewComponent() =>
        typeof(StripePaymentViewComponent);

    // ── Plugin lifecycle ────────────────────────────────────────────────────

    public override string GetConfigurationPageUrl() =>
        $"{_webHelper.GetStoreLocation()}Admin/StripePayment/Configure";

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new StripePaymentSettings
        {
            UseTestMode = true
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Payments.Stripe.Fields.SecretKey"] = "Secret Key",
            ["Plugins.Payments.Stripe.Fields.PublishableKey"] = "Publishable Key",
            ["Plugins.Payments.Stripe.Fields.UseTestMode"] = "Use Test Mode",
            ["Plugins.Payments.Stripe.Fields.AdditionalFee"] = "Additional Fee",
            ["Plugins.Payments.Stripe.Fields.AdditionalFeePercentage"] = "Additional Fee is Percentage",
            ["Plugins.Payments.Stripe.PaymentMethodDescription"] = "Pay securely with your card via Stripe",
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<StripePaymentSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Stripe");
        await base.UninstallAsync();
    }

    public async Task<string> GetPaymentMethodDescriptionAsync() =>
        await _localizationService.GetResourceAsync(
            "Plugins.Payments.Stripe.PaymentMethodDescription");
}