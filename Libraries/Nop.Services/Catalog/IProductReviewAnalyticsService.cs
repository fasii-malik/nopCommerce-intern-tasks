using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

 public partial interface IProductReviewAnalyticsService
    {
    //MY CUSTOM FOR TASK#1
    /// <summary>
    /// Gets approved product reviews by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of approved product reviews
    /// </returns>
    Task<IList<ProductReview>> GetProductReviewStatsAsync(int productId);
}
