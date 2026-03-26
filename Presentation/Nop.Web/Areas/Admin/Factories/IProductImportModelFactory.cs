using System.Threading.Tasks;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface IProductImportModelFactory
    {
        /// <summary>
        /// Prepare product import model
        /// </summary>
        /// <returns>Product import model</returns>
        Task<ProductImportModel> PrepareProductImportModelAsync();

        /// <summary>
        /// Prepare import result model
        /// </summary>
        /// <param name="result">Import result from service</param>
        /// <returns>Import result model</returns>
        Task<ImportResultModel> PrepareImportResultModelAsync(ProductImportResult result);
    }
}