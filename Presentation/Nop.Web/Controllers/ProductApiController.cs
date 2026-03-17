using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Models.Api;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers
{
    //[Area("")]
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : Controller
    {
        private readonly IProductService _productService;

        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.SearchProductsAsync(
                pageSize: int.MaxValue,
                visibleIndividuallyOnly: true
            );

            var result = products.Select(p => new ProductApiModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();

            return Json(result);
        }
    }
}