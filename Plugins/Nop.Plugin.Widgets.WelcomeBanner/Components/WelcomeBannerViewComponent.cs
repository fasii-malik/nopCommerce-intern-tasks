using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.WelcomeBanner.Components
{
    [ViewComponent(Name = "WelcomeBanner")]
    public class WelcomeBannerViewComponent : NopViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            // Task.FromResult allows you to return the view immediately in an async method
            return await Task.FromResult(View("~/Plugins/Widgets.WelcomeBanner/Views/Components/WelcomeBanner/Default.cshtml"));
        }
    }
}