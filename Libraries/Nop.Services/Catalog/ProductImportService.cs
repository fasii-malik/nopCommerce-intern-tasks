using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Services.Logging;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    public partial class ProductImportService : IProductImportService
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILogger _logger;

        public ProductImportService(
            IProductService productService,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ILogger logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _logger = logger;
        }

        public async Task<List<ProductCsvImportModel>> ParseCsvAsync(Stream fileStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, config);

            var records = new List<ProductCsvImportModel>();

            await foreach (var record in csv.GetRecordsAsync<CsvRecord>())
            {
                records.Add(new ProductCsvImportModel
                {
                    Name = record.Name,
                    Price = record.Price,
                    Category = record.Category,
                    Stock = record.Stock
                });
            }

            return records;
        }

        public async Task<ProductImportResult> ImportProductsAsync(List<ProductCsvImportModel> csvModels)
        {
            var result = new ProductImportResult
            {
                TotalProcessed = csvModels.Count
            };

            foreach (var csvModel in csvModels)
            {
                try
                {
                    // Validate
                    if (string.IsNullOrWhiteSpace(csvModel.Name))
                    {
                        result.ErrorCount++;
                        result.ErrorMessages.Add("Row skipped: Product name is required.");
                        continue;
                    }

                    if (csvModel.Price <= 0)
                    {
                        result.ErrorCount++;
                        result.ErrorMessages.Add($"Row '{csvModel.Name}': Price must be greater than 0.");
                        continue;
                    }

                    if (csvModel.Stock < 0)
                    {
                        result.ErrorCount++;
                        result.ErrorMessages.Add($"Row '{csvModel.Name}': Stock cannot be negative.");
                        continue;
                    }

                    // Find or create category
                    int categoryId = 0;
                    if (!string.IsNullOrWhiteSpace(csvModel.Category))
                    {
                        var category = (await _categoryService.GetAllCategoriesAsync(categoryName: csvModel.Category))
                            .FirstOrDefault();

                        if (category == null)
                        {
                            // Create new category
                            category = new Category
                            {
                                Name = csvModel.Category,
                                Published = true,
                                DisplayOrder = 0,
                                CreatedOnUtc = DateTime.UtcNow,
                                UpdatedOnUtc = DateTime.UtcNow
                            };
                            await _categoryService.InsertCategoryAsync(category);

                            // Create SEO-friendly URL
                            var seName = await _urlRecordService.ValidateSeNameAsync(category, string.Empty, category.Name, true);
                            await _urlRecordService.SaveSlugAsync(category, seName, 0);
                        }

                        categoryId = category.Id;
                    }

                    // Create product
                    var product = new Product
                    {
                        Name = csvModel.Name,
                        Price = csvModel.Price,
                        StockQuantity = csvModel.Stock,
                        Published = true,
                        VisibleIndividually = true,
                        ProductTypeId = (int)ProductType.SimpleProduct,
                        ManageInventoryMethodId = (int)ManageInventoryMethod.ManageStock,
                        OrderMinimumQuantity = 1,
                        OrderMaximumQuantity = 10000,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    await _productService.InsertProductAsync(product);

                    // Associate with category
                    if (categoryId > 0)
                    {
                        var productCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId,
                            DisplayOrder = 0
                        };
                        await _categoryService.InsertProductCategoryAsync(productCategory);
                    }

                    // Create SEO-friendly URL for product
                    var productSeName = await _urlRecordService.ValidateSeNameAsync(product, string.Empty, product.Name, true);
                    await _urlRecordService.SaveSlugAsync(product, productSeName, 0);

                    result.SuccessCount++;
                    result.SuccessMessages.Add($"✓ Product '{csvModel.Name}' imported successfully.");

                    await _logger.InformationAsync($"Product imported: {csvModel.Name} (ID: {product.Id})");
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.ErrorMessages.Add($"✗ Error importing '{csvModel.Name}': {ex.Message}");
                    await _logger.ErrorAsync($"Product import failed for '{csvModel.Name}'", ex);
                }
            }

            return result;
        }

        // Helper class for CsvHelper mapping
        private class CsvRecord
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int Stock { get; set; }
        }
    }
}