using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Misc.WarrantySystem.Components
{
    [ViewComponent(Name = "CustomerNavigationWidget")]
    public class CustomerNavigationWidget : NopViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            // Use string literal instead of PublicWidgetZones.CustomerNavigation
            if (widgetZone == "account_navigation_after")
            {
                return View("~/Plugins/Misc.WarrantySystem/Views/Widgets/CustomerNavigation.cshtml");
            }

            return Content(string.Empty);
        }
    }
}