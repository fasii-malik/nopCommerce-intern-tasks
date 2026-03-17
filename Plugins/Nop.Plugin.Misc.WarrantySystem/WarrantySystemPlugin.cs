using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Web.Framework.Menu;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Nop.Plugin.Misc.WarrantySystem;

public class WarrantySystemPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
{
    public bool HideInWidgetList => false;
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>()); // empty 
        //return Task.FromResult<IList<string>>(new List<string>
        //{
        //    "account_navigation_after"
        //});
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(Components.CustomerNavigationWidget);
    }

    public Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var warrantyMenuItem = new SiteMapNode()
        {
            SystemName = "WarrantySystem",
            Title = "Warranty Registrations",
            ControllerName = "AdminWarranty",
            ActionName = "List",
            Visible = true,
            IconClass = "far fa-dot-circle",
            RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
        };

        var customerNode = rootNode.ChildNodes
            .FirstOrDefault(x => x.SystemName == "Sales");

        if (customerNode != null)
            customerNode.ChildNodes.Add(warrantyMenuItem);

        return Task.CompletedTask;
    }

}