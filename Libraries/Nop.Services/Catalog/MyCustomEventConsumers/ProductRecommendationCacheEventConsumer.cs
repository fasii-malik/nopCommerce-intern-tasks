using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Core.Caching;
using Nop.Services.Events;

namespace Nop.Services.Catalog.MyCustomEventConsumers;
/// <summary>
/// Invalidates FBT cache when products are updated or deleted.
/// No need to touch ProductService at all.
/// </summary>
public class ProductRecommendationCacheEventConsumer :
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityDeletedEvent<Product>>
{
    private readonly IStaticCacheManager _staticCacheManager;

    public ProductRecommendationCacheEventConsumer(
        IStaticCacheManager staticCacheManager)
    {
        _staticCacheManager = staticCacheManager;
    }

    // Fires automatically when any product is updated
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(
            ProductRecommendationService.FrequentlyBoughtTogetherCacheKey,
            eventMessage.Entity.Id);
    }

    // Fires automatically when any product is deleted
    public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(
            ProductRecommendationService.FrequentlyBoughtTogetherCacheKey,
            eventMessage.Entity.Id);
    }
}
