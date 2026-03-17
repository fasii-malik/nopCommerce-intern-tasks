using System;

namespace Nop.Web.Areas.Admin.Models.Catalog;

    /// <summary>
    /// View model holding computed review statistics for display on the Admin Product Details page.
    /// </summary>
    public class ProductReviewStatsModel
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public double FiveStarPercentage { get; set; }
        public DateTime? LatestReviewDate { get; set; }
    }
