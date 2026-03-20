using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

public record DailyOrderReportModel : BaseNopModel
{
    public DateTime ReportDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TopSellingProductName { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}