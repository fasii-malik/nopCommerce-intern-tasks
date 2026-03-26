using System.Threading.Tasks;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class ProductImportModelFactory : IProductImportModelFactory
    {
        public Task<ProductImportModel> PrepareProductImportModelAsync()
        {
            var model = new ProductImportModel();
            return Task.FromResult(model);
        }

        public Task<ImportResultModel> PrepareImportResultModelAsync(ProductImportResult result)
        {
            var model = new ImportResultModel
            {
                TotalProcessed = result.TotalProcessed,
                SuccessCount = result.SuccessCount,
                ErrorCount = result.ErrorCount,
                SuccessMessages = result.SuccessMessages,
                ErrorMessages = result.ErrorMessages
            };

            return Task.FromResult(model);
        }
    }
}