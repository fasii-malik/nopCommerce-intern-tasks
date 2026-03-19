using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Services.Catalog.MyCustomEventConsumers;
public class ProductReviewAnalyticsCacheEventConsumer :
        IConsumer<EntityInsertedEvent<ProductReview>>,
        IConsumer<EntityUpdatedEvent<ProductReview>>,
        IConsumer<EntityDeletedEvent<ProductReview>>
{
    private readonly IStaticCacheManager _staticCacheManager;

    public ProductReviewAnalyticsCacheEventConsumer(IStaticCacheManager staticCacheManager)
    {
        _staticCacheManager = staticCacheManager;
    }

    // New review added — invalidate cache for that product
    public async Task HandleEventAsync(EntityInsertedEvent<ProductReview> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(
            ProductReviewAnalyticsService.ProductReviewStatsCacheKey,
            eventMessage.Entity.ProductId);
    }

    // Review updated (e.g. approved/rejected) — invalidate
    public async Task HandleEventAsync(EntityUpdatedEvent<ProductReview> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(
            ProductReviewAnalyticsService.ProductReviewStatsCacheKey,
            eventMessage.Entity.ProductId);
    }

    // Review deleted — invalidate
    public async Task HandleEventAsync(EntityDeletedEvent<ProductReview> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(
            ProductReviewAnalyticsService.ProductReviewStatsCacheKey,
            eventMessage.Entity.ProductId);
    }
}
