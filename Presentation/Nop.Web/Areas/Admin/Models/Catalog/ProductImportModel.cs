using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record ProductImportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.ProductImport.Fields.CsvFile")]
        public IFormFile CsvFile { get; set; }
    }
}