using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.WarrantySystem.Domain;
using Nop.Plugin.Misc.WarrantySystem.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.WarrantySystem.Services
{
    public class WarrantyService : IWarrantyService
    {
        private readonly IRepository<WarrantyRegistration> _repo;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        public WarrantyService(IRepository<WarrantyRegistration> repo, IWorkContext workContext, IStoreContext storeContext, IProductService productService)
        {
            _repo = repo;
            _workContext = workContext;
            _storeContext = storeContext;
            _productService = productService;
        }

        public async Task InsertWarrantyRegistrationAsync(WarrantyRegisterModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var entity = new WarrantyRegistration
            {
                CustomerId = customer.Id,
                ProductName = model.SelectedProductName,
                SerialNumber = model.SerialNumber,
                InvoicePictureId = null,
                Status = 0,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _repo.InsertAsync(entity);
        }

        public IList<SelectListItem> GetAllProductsSelectList()
        {
            // placeholder: later fetch from products table
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Product 1", Value = "Product 1" },
                new SelectListItem { Text = "Product 2", Value = "Product 2" }
            };
        }
        // get all the products
        public async Task<IList<SelectListItem>> GetAllProductsSelectListAsync()
        {
            // get current context
            // get current store
            var store = await _storeContext.GetCurrentStoreAsync();
            var storeId = store?.Id ?? 0;
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

            // fetch products
            var productsPagedList = await _productService.SearchProductsAsync(
                pageIndex: 0,
                pageSize: 100,
                storeId: storeId,
                visibleIndividuallyOnly: true,
                languageId: languageId,
                showHidden: false // only published
            );

            var products = productsPagedList?.ToList() ?? new List<Product>();

            // map to SelectListItem
            var selectList = products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Name
            }).ToList();            

            return selectList;
        }
        public IList<WarrantyRegistration> GetAllWarranties()
        {
            return _repo.Table.ToList();
        }

        public void UpdateStatus(int id, int status)
        {
            var entity = _repo.GetById(id);
            if (entity == null)
                return;
            entity.Status = status;
            _repo.Update(entity);
        }
    }
}