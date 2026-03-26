using System.Diagnostics;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;        // ← NopCommerce's own ILogger
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Services.Catalog;

public class SlowQueryProductService : ProductService
{
    private const int SlowThresholdMs = 0; // ← keep at 0 until confirmed working, then set to 200
    private readonly ILogger _nopLogger;   // ← Nop.Services.Logging.ILogger, NOT Microsoft's

    public SlowQueryProductService(
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        IAclService aclService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IRepository<Category> categoryRepository,
        IRepository<CrossSellProduct> crossSellProductRepository,
        IRepository<DiscountProductMapping> discountProductMappingRepository,
        IRepository<LocalizedProperty> localizedPropertyRepository,
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<Product> productRepository,
        IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
        IRepository<ProductAttributeMapping> productAttributeMappingRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductPicture> productPictureRepository,
        IRepository<ProductProductTagMapping> productTagMappingRepository,
        IRepository<ProductReview> productReviewRepository,
        IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
        IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
        IRepository<ProductTag> productTagRepository,
        IRepository<ProductVideo> productVideoRepository,
        IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
        IRepository<RelatedProduct> relatedProductRepository,
        IRepository<Shipment> shipmentRepository,
        IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
        IRepository<TierPrice> tierPriceRepository,
        ISearchPluginManager searchPluginManager,
        IStaticCacheManager staticCacheManager,
        IStoreService storeService,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        ILogger nopLogger)              // ← replaced ILogger<SlowQueryProductService>
        : base(
            catalogSettings,
            commonSettings,
            aclService,
            customerService,
            dateRangeService,
            languageService,
            localizationService,
            productAttributeParser,
            productAttributeService,
            categoryRepository,
            crossSellProductRepository,
            discountProductMappingRepository,
            localizedPropertyRepository,
            manufacturerRepository,
            productRepository,
            productAttributeCombinationRepository,
            productAttributeMappingRepository,
            productCategoryRepository,
            productManufacturerRepository,
            productPictureRepository,
            productTagMappingRepository,
            productReviewRepository,
            productReviewHelpfulnessRepository,
            productSpecificationAttributeRepository,
            productTagRepository,
            productVideoRepository,
            productWarehouseInventoryRepository,
            relatedProductRepository,
            shipmentRepository,
            stockQuantityHistoryRepository,
            tierPriceRepository,
            searchPluginManager,
            staticCacheManager,
            storeService,
            storeMappingService,
            workContext,
            localizationSettings)
    {
        _nopLogger = nopLogger;
    }

    public override async Task<IPagedList<Product>> SearchProductsAsync(
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        int storeId = 0,
        int vendorId = 0,
        int warehouseId = 0,
        ProductType? productType = null,
        bool visibleIndividuallyOnly = false,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        int languageId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null,
        ProductSortingEnum orderBy = ProductSortingEnum.Position,
        bool showHidden = false,
        bool? overridePublished = null)
    {
        var sw = Stopwatch.StartNew();

        var result = await base.SearchProductsAsync(
            pageIndex, pageSize, categoryIds, manufacturerIds,
            storeId, vendorId, warehouseId, productType,
            visibleIndividuallyOnly, excludeFeaturedProducts,
            priceMin, priceMax, productTagId, keywords,
            searchDescriptions, searchManufacturerPartNumber,
            searchSku, searchProductTags, languageId,
            filteredSpecOptions, orderBy, showHidden, overridePublished);

        sw.Stop();

        var message = $"[PerfOptimizer] SearchProductsAsync {sw.ElapsedMilliseconds}ms | " +
                      $"page={pageIndex} size={pageSize} keywords='{keywords ?? "(none)"}' " +
                      $"categories=[{(categoryIds != null ? string.Join(",", categoryIds) : "all")}] " +
                      $"storeId={storeId} orderBy={orderBy}";

        if (sw.ElapsedMilliseconds >= SlowThresholdMs)
        {
            // ← InformationAsync so it shows in Admin → System → Log at Info level
            await _nopLogger.InformationAsync(message);
        }

        return result;
    }
}