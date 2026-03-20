using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Services.Orders;

public class DailyOrderReportService : IDailyOrderReportService
{
    private readonly IRepository<DailyOrderReport> _reportRepository;

    public DailyOrderReportService(IRepository<DailyOrderReport> reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task InsertReportAsync(DailyOrderReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        await _reportRepository.InsertAsync(report);
    }

    public async Task<bool> ReportExistsForDateAsync(DateTime date)
    {
        var dateOnly = date.Date;
        return await _reportRepository.Table
            .AnyAsync(r => r.ReportDate == dateOnly);
    }

    public async Task<IList<DailyOrderReport>> GetReportsAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _reportRepository.Table;

        if (from.HasValue)
            query = query.Where(r => r.ReportDate >= from.Value.Date);
        if (to.HasValue)
            query = query.Where(r => r.ReportDate <= to.Value.Date);

        return await query.OrderByDescending(r => r.ReportDate).ToListAsync();
    }
}