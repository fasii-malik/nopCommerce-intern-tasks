using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductImportController : BaseAdminController
    {
        private readonly IProductImportService _productImportService;
        private readonly IProductImportModelFactory _productImportModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public ProductImportController(
            IProductImportService productImportService,
            IProductImportModelFactory productImportModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _productImportService = productImportService;
            _productImportModelFactory = productImportModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public virtual async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = await _productImportModelFactory.PrepareProductImportModelAsync();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import(ProductImportModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductImport.NoFile"));
                return RedirectToAction("Index");
            }

            if (!model.CsvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductImport.InvalidFileType"));
                return RedirectToAction("Index");
            }

            try
            {
                // Parse CSV
                using var stream = model.CsvFile.OpenReadStream();
                var csvModels = await _productImportService.ParseCsvAsync(stream);

                if (!csvModels.Any())
                {
                    _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductImport.NoData"));
                    return RedirectToAction("Index");
                }

                // Import products
                var result = await _productImportService.ImportProductsAsync(csvModels);

                // Prepare result model
                var resultModel = await _productImportModelFactory.PrepareImportResultModelAsync(result);

                return View("ImportResult", resultModel);
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification($"Import failed: {ex.Message}");
                return RedirectToAction("Index");
            }
        }
    }
}