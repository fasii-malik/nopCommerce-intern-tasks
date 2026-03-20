using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class DailyOrderReportController : BaseAdminController
{
    private readonly IDailyOrderReportService _dailyOrderReportService;

    public DailyOrderReportController(IDailyOrderReportService dailyOrderReportService)
    {
        _dailyOrderReportService = dailyOrderReportService;
    }

    public async Task<IActionResult> Index()
    {
        var reports = await _dailyOrderReportService.GetReportsAsync();

        var models = reports.Select(r => new DailyOrderReportModel
        {
            ReportDate = r.ReportDate,
            TotalOrders = r.TotalOrders,
            TotalRevenue = r.TotalRevenue,
            TopSellingProductName = r.TopSellingProductName,
            CreatedOnUtc = r.CreatedOnUtc
        }).ToList();

        return View(models);
    }
}