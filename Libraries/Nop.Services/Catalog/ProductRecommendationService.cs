using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Services.Catalog
{
    public partial class ProductRecommendationService : IProductRecommendationService
    {
        #region Fields

        protected readonly IRepository<OrderItem> _orderItemRepository;
        protected readonly IRepository<Product> _productRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILogger _logger;

        // Cache key template — {0} will be replaced with productId at runtime
        internal static readonly CacheKey FrequentlyBoughtTogetherCacheKey =
     new("nop.product.frequentlyboughttogether.{0}",
         "nop.product.frequentlyboughttogether.");
        #endregion

        #region Ctor

        public ProductRecommendationService(
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            ILogger logger,
            IStaticCacheManager staticCacheManager)
        {
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _logger = logger;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        public async Task<IList<Product>> GetFrequentlyBoughtTogetherAsync(int productId)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                FrequentlyBoughtTogetherCacheKey, productId);

            // Log EVERY call to this method
            await _logger.InformationAsync($"[FBT] Method called for productId={productId}");

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                // This only logs on a CACHE MISS (DB hit)
                await _logger.InformationAsync($"[FBT] CACHE MISS — querying DB for productId={productId}");

                // Step 1: find all orderIds that contain this product
                var orderItemsForProduct = await _orderItemRepository.GetAllAsync(query =>
                    query.Where(oi => oi.ProductId == productId));

                if (!orderItemsForProduct.Any())
                    return new List<Product>();

                var orderIds = orderItemsForProduct.Select(oi => oi.OrderId).ToList();

                // Step 2: find all OTHER items in those orders
                var relatedItems = await _orderItemRepository.GetAllAsync(query =>
                    query.Where(oi => orderIds.Contains(oi.OrderId) && oi.ProductId != productId));

                if (!relatedItems.Any())
                    return new List<Product>();

                // Step 3: count occurrences and take top 5 productIds
                var topProductIds = relatedItems
                    .GroupBy(oi => oi.ProductId)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList();

                // Step 4: fetch the actual product entities
                var products = await _productRepository.GetAllAsync(query =>
                    query.Where(p => topProductIds.Contains(p.Id) && !p.Deleted && p.Published));

                await _logger.InformationAsync($"[FBT] DB query complete, caching result for productId={productId}");

                return products;
            });
        }

        #endregion
    }
}