using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Orders.MyCustomTasks;
/// <summary>
/// Scheduled task: generates a daily order report once per day.
/// Registered in the ScheduleTask table with RunPeriod = 1440 (minutes).
/// </summary>
public class DailyOrderReportTask : IScheduleTask
{
    private readonly IOrderService _orderService;
    private readonly IDailyOrderReportService _dailyOrderReportService;

    public DailyOrderReportTask(
        IOrderService orderService,
        IDailyOrderReportService dailyOrderReportService)
    {
        _orderService = orderService;
        _dailyOrderReportService = dailyOrderReportService;
    }

    public async Task ExecuteAsync()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        // Idempotency guard — don't re-generate if already done today
        if (await _dailyOrderReportService.ReportExistsForDateAsync(yesterday))
            return;

        // Pull completed orders for yesterday
        var orders = await _orderService.SearchOrdersAsync(
            createdFromUtc: yesterday,
            createdToUtc: yesterday.AddDays(1).AddSeconds(-1)
            //osIds: new List<int> { (int)OrderStatus.Complete } //it will only return completed order, i commented it for testing.
            );

        if (!orders.Any())
        {
            // Still persist a zero-record so the guard doesn't re-query forever
            await _dailyOrderReportService.InsertReportAsync(new DailyOrderReport
            {
                ReportDate = yesterday,
                TotalOrders = 0,
                TotalRevenue = 0,
                TopSellingProductId = 0,
                TopSellingProductName = "N/A",
                CreatedOnUtc = DateTime.UtcNow
            });
            return;
        }

        // Total orders & revenue
        var totalOrders = orders.TotalCount;
        var totalRevenue = orders.Sum(o => o.OrderTotal);

        // Top selling product — flatten all order items
        var allItems = new List<OrderItem>();
        foreach (var order in orders)
            allItems.AddRange(await _orderService.GetOrderItemsAsync(order.Id));

        var topItem = allItems
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.Qty)
            .FirstOrDefault();

        var topProductId = topItem?.ProductId ?? 0;
        var topProductName = "N/A";

        if (topProductId > 0)
        {
            // ProductService is available via DI — inject if needed, or resolve via IProductService
            // For brevity we store the ID; wire up IProductService if you want the name at generation time
            topProductName = $"ProductId:{topProductId}";
        }

        var report = new DailyOrderReport
        {
            ReportDate = yesterday,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TopSellingProductId = topProductId,
            TopSellingProductName = topProductName,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _dailyOrderReportService.InsertReportAsync(report);
    }
}