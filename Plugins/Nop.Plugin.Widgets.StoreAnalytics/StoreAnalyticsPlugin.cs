using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.StoreAnalytics
{
    public class StoreAnalyticsPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "StoreAnalytics";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                AdminWidgetZones.DashboardTop
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(Components.StoreAnalyticsViewComponent);
        }
    }
}