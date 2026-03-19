using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers.Api
{
    [Route("api/product-search")]
    [ApiController]
    public class ProductSearchApiController : BaseController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructor

        public ProductSearchApiController(
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Search products with multiple filters and pagination.
        /// GET /api/products/search
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? manufacturerId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate pagination parameters
            if (page < 1)
                page = 1;

            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            // Validate price range
            if (minPrice.HasValue && minPrice < 0)
                return BadRequest(new { error = "minPrice cannot be negative." });

            if (maxPrice.HasValue && maxPrice < 0)
                return BadRequest(new { error = "maxPrice cannot be negative." });

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest(new { error = "minPrice cannot be greater than maxPrice." });

            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();

            // SearchProductsAsync uses 0-based pageIndex internally
            var pageIndex = page - 1;

            // Use NopCommerce's built-in search (handles ACL, store mapping, published state)
            var products = await _productService.SearchProductsAsync(
                pageIndex: pageIndex,
                pageSize: pageSize,
                categoryIds: categoryId.HasValue ? new List<int> { categoryId.Value } : null,
                manufacturerIds: manufacturerId.HasValue ? new List<int> { manufacturerId.Value } : null,
                storeId: store.Id,
                keywords: keyword,
                searchDescriptions: false,
                searchManufacturerPartNumber: false,
                searchSku: false,
                priceMin: minPrice,
                priceMax: maxPrice,
                visibleIndividuallyOnly: true,
                overridePublished: null          // only published products
            );

            // Build the result items
            var items = products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                shortDescription = p.ShortDescription,
                sku = p.Sku,
                price = p.Price,
                oldPrice = p.OldPrice > 0 ? (decimal?)p.OldPrice : null,
                stockQuantity = p.StockQuantity,
                published = p.Published,
                createdOnUtc = p.CreatedOnUtc
            }).ToList();

            var response = new
            {
                success = true,
                page,
                pageSize,
                totalCount = products.TotalCount,
                totalPages = (int)Math.Ceiling(products.TotalCount / (double)pageSize),
                filters = new
                {
                    keyword,
                    categoryId,
                    manufacturerId,
                    minPrice,
                    maxPrice
                },
                results = items
            };

            return Ok(response);
        }

        #endregion
    }
}