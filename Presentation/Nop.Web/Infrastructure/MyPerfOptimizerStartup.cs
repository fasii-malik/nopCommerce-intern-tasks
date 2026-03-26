using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Catalog;

namespace Nop.Web.Infrastructure;

public class PerfOptimizerStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IProductService));
        if (descriptor != null)
            services.Remove(descriptor);

        services.AddScoped<IProductService, SlowQueryProductService>();
    }

    public void Configure(IApplicationBuilder application) { }

    public int Order => 3000;
}