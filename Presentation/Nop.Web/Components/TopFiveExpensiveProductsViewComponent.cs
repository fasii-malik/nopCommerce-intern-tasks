using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public class TopFiveExpensiveProductsViewComponent : NopViewComponent
{
    private readonly IProductService _productService;
    private readonly IProductModelFactory _productModelFactory;

    public TopFiveExpensiveProductsViewComponent(
        IProductService productService,
        IProductModelFactory productModelFactory)
    {
        _productService = productService;
        _productModelFactory = productModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var products = await _productService.GetTopMostExpensiveProductsAsync(5);

        var models = await _productModelFactory.PrepareProductOverviewModelsAsync(products);

        return View(models);
    }
}