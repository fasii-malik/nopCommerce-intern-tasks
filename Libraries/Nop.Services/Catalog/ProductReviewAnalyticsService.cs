using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Services.Catalog
{
    public partial class ProductReviewAnalyticsService : IProductReviewAnalyticsService
    {
        #region Fields

        protected readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILogger _logger;

        internal static readonly CacheKey ProductReviewStatsCacheKey =
            new("nop.product.reviewstats.{0}",
                "nop.product.reviewstats.");

        #endregion

        #region Ctor

        public ProductReviewAnalyticsService(
            IRepository<ProductReview> productReviewRepository,
            IStaticCacheManager staticCacheManager,
            ILogger logger)
        {
            _productReviewRepository = productReviewRepository;
            _staticCacheManager = staticCacheManager;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<IList<ProductReview>> GetProductReviewStatsAsync(int productId)
        {
            await _logger.InformationAsync($"[ReviewStats] Method called for productId={productId}");

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                ProductReviewStatsCacheKey, productId);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                await _logger.InformationAsync($"[ReviewStats] CACHE MISS — querying DB for productId={productId}");

                var result = await _productReviewRepository.GetAllAsync(query =>
                    query.Where(r => r.ProductId == productId && r.IsApproved));

                await _logger.InformationAsync($"[ReviewStats] DB query complete — {result.Count} reviews cached for productId={productId}");

                return result;
            });
        }

        #endregion
    }
}