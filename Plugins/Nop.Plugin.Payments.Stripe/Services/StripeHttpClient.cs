using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Payments.Stripe.Services;

public class StripeHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly StripePaymentSettings _settings;

    public StripeHttpClient(HttpClient httpClient, StripePaymentSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;

        // ── IMPORTANT: BaseAddress must end with a trailing slash ─────────
        // And the endpoint must NOT start with a slash
        // HttpClient rule: if BaseAddress = https://api.stripe.com/v1/
        //                  and you call    payment_intents
        //                  result =        https://api.stripe.com/v1/payment_intents ✓
        //
        // If BaseAddress = https://api.stripe.com/v1  (no trailing slash)
        //                  and you call /payment_intents
        //                  result =     https://api.stripe.com/payment_intents  ✗ (v1 stripped!)

        _httpClient.BaseAddress = new Uri("https://api.stripe.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.SecretKey);
    }

    public async Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        long amount, string currency, string paymentMethodId)
    {
        var formData = new Dictionary<string, string>
        {
            ["amount"] = amount.ToString(),
            ["currency"] = currency.ToLowerInvariant(),
            ["payment_method"] = paymentMethodId,
            ["confirm"] = "true",
            ["return_url"] = "https://localhost:53172/checkout/completed"
        };

        // ── No leading slash on the endpoint ─────────────────────────────
        var response = await _httpClient.PostAsync(
            "payment_intents",
            new FormUrlEncodedContent(formData));

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new NopException($"Stripe API error: {json}");

        return JsonSerializer.Deserialize<StripePaymentIntentResult>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}

public class StripePaymentIntentResult
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public StripeLastPaymentError? LastPaymentError { get; set; }
}

public class StripeLastPaymentError
{
    public string Message { get; set; } = string.Empty;
}
