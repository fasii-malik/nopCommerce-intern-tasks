using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;
public partial interface IProductRecommendationService
{
    /// <summary>
    /// Gets top 5 frequently bought together products
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of products
    /// </returns>
    Task<IList<Product>> GetFrequentlyBoughtTogetherAsync(int productId);
    
}
