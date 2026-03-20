using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Orders;
public class DailyOrderReport : BaseEntity
{
    public DateTime ReportDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TopSellingProductId { get; set; }
    public string TopSellingProductName { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
