using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Web.Framework.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;

namespace Nop.Plugin.Widgets.WelcomeBanner
{
    public class WelcomeBannerPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        // ✅ Correct method name for NopCommerce 4.70
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(Components.WelcomeBannerViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.HomepageTop
            });
        }
    }
}