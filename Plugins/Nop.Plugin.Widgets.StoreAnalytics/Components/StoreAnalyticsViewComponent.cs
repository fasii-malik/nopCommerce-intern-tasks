using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.StoreAnalytics.Models;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.StoreAnalytics.Components
{
    public class StoreAnalyticsViewComponent : NopViewComponent
    {
        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;

        public StoreAnalyticsViewComponent(
            IOrderService orderService,
            IOrderReportService orderReportService,
            ICustomerService customerService,
            IStoreContext storeContext)
        {
            _orderService = orderService;
            _orderReportService = orderReportService;
            _customerService = customerService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            // Today's date boundaries (UTC)
            var todayStart = DateTime.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1);

            // 1. Total Orders Today
            var todaysOrders = await _orderService.SearchOrdersAsync(
                storeId: store.Id,
                createdFromUtc: todayStart,
                createdToUtc: todayEnd,
                //It will only show Processing and Complete order.
                osIds: new[] { (int)OrderStatus.Processing, (int)OrderStatus.Complete }.ToList()
            );
            // After — all statuses included
            //var todaysOrders = await _orderService.SearchOrdersAsync(
            //    storeId: store.Id,
            //    createdFromUtc: todayStart,
            //    createdToUtc: todayEnd
            //);

            // 2. Total Revenue Today
            var revenueToday = todaysOrders.Sum(o => o.OrderTotal);

            // 3. New Customers Today
            // Filter by Registered role to exclude guests (IsGuest() is not available in 4.70.5)
            var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            var newCustomers = await _customerService.GetAllCustomersAsync(
                customerRoleIds: new[] { registeredRole.Id },
                createdFromUtc: todayStart,
                createdToUtc: todayEnd
            );
            var newCustomerCount = newCustomers.TotalCount;

            // 4. Top Selling Product
            // BestSellersReportAsync is on IOrderReportService, not IOrderService
            var bestSellers = await _orderReportService.BestSellersReportAsync(
                storeId: store.Id,
                orderBy: OrderByEnum.OrderByTotalAmount,
                pageSize: 1
            );

            var topProductName = "N/A";
            var topProductQty = 0;

            var topItem = bestSellers.FirstOrDefault();
            if (topItem != null)
            {
                topProductName = topItem.ProductName;
                topProductQty = topItem.TotalQuantity;
            }

            var model = new StoreAnalyticsModel
            {
                TotalOrdersToday = todaysOrders.TotalCount,
                TotalRevenueToday = revenueToday,
                NewCustomersToday = newCustomerCount,
                TopSellingProductName = topProductName,
                TopSellingProductQuantity = topProductQty
            };

            return View("~/Plugins/Widgets.StoreAnalytics/Views/StoreAnalytics/Default.cshtml", model);
        }
    }
}