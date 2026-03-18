using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public partial class FrequentlyBoughtTogetherViewComponent : NopViewComponent
    {
        protected readonly IProductRecommendationService _productRecommendationService;
        protected readonly IProductModelFactory _productModelFactory;

        public FrequentlyBoughtTogetherViewComponent(
            IProductRecommendationService productRecommendationService,
            IProductModelFactory productModelFactory)
        {
            _productRecommendationService = productRecommendationService;
            _productModelFactory = productModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId, int? productThumbPictureSize)
        {
            var products = await _productRecommendationService.GetFrequentlyBoughtTogetherAsync(productId);

            if (!products.Any())
                return Content(string.Empty);

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();

            return View(model);
        }
    }
}
