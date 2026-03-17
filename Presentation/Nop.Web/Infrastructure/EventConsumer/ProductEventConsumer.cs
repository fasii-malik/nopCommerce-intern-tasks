using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Logging;  // ← keep this

namespace Nop.Web.Infrastructure.EventConsumer
{
    public class ProductEventConsumer :
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>
    {
        private readonly Nop.Services.Logging.ILogger _logger;  // ← fully qualified

        public ProductEventConsumer(Nop.Services.Logging.ILogger logger)  // ← fully qualified
        {
            _logger = logger;
        }

        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            var product = eventMessage.Entity;
            await _logger.InformationAsync($"Product created with ID: {product.Id}");
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            var product = eventMessage.Entity;
            await _logger.InformationAsync($"Product updated with ID: {product.Id}");
        }
    }
}