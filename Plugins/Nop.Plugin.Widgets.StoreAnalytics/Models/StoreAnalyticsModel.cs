using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.StoreAnalytics.Models;
    public class StoreAnalyticsModel
    {
        public int TotalOrdersToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public int NewCustomersToday { get; set; }
        public string TopSellingProductName { get; set; }
        public int TopSellingProductQuantity { get; set; }
    }

