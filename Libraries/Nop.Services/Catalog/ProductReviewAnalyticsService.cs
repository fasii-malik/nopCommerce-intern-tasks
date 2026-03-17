using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    //MY CUSTOM FOR TASK#1
    public partial class ProductReviewAnalyticsService : IProductReviewAnalyticsService
    {
        #region Fields

        protected readonly IRepository<ProductReview> _productReviewRepository;

        #endregion

        #region Ctor

        public ProductReviewAnalyticsService(IRepository<ProductReview> productReviewRepository)
        {
            _productReviewRepository = productReviewRepository;
        }

        #endregion

        #region Methods

        public async Task<IList<ProductReview>> GetProductReviewStatsAsync(int productId)
        {
            return await _productReviewRepository.GetAllAsync(query =>
                query.Where(r => r.ProductId == productId && r.IsApproved));
        }

        #endregion
    }
}