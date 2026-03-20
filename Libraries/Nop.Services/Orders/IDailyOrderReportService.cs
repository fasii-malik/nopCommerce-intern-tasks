using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders;

public interface IDailyOrderReportService
{
    Task InsertReportAsync(DailyOrderReport report);
    Task<bool> ReportExistsForDateAsync(DateTime date);
    Task<IList<DailyOrderReport>> GetReportsAsync(DateTime? from = null, DateTime? to = null);
}