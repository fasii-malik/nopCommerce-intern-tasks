using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial interface IProductImportService
    {
        /// <summary>
        /// Parse CSV file and return list of product models
        /// </summary>
        /// <param name="fileStream">CSV file stream</param>
        /// <returns>List of product CSV models</returns>
        Task<List<ProductCsvImportModel>> ParseCsvAsync(Stream fileStream);

        /// <summary>
        /// Import products from CSV models
        /// </summary>
        /// <param name="csvModels">List of CSV models</param>
        /// <returns>Import result with success/error counts</returns>
        Task<ProductImportResult> ImportProductsAsync(List<ProductCsvImportModel> csvModels);
    }

    /// <summary>
    /// Product CSV import model
    /// </summary>
    public class ProductCsvImportModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
    }

    /// <summary>
    /// Product import result
    /// </summary>
    public class ProductImportResult
    {
        public ProductImportResult()
        {
            SuccessMessages = new List<string>();
            ErrorMessages = new List<string>();
        }

        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> SuccessMessages { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}