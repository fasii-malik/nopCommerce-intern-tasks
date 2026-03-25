// Infrastructure/NopStartup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Stripe.Services;

namespace Nop.Plugin.Payments.Stripe.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register typed HttpClient for Stripe
        services.AddHttpClient<StripeHttpClient>();
    }

    public void Configure(IApplicationBuilder application) { }

    public int Order => 3000;
}