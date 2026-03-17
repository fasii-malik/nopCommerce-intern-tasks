using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.WarrantySystem.Models;
using Nop.Plugin.Misc.WarrantySystem.Services;
using System.Threading.Tasks;
using System;

namespace Nop.Plugin.Misc.WarrantySystem.Controllers
{
    public class WarrantyController : BasePluginController
    {
        private readonly ICustomerService _customerService;
        private readonly IWarrantyService _warrantyService;

        public WarrantyController(ICustomerService customerService, IWarrantyService warrantyService)
        {
            _customerService = customerService;
            _warrantyService = warrantyService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new WarrantyRegisterModel
            {
                Products = await _warrantyService.GetAllProductsSelectListAsync()
            };
            Console.WriteLine("\n\n"+model.SelectedProductId);
            Console.WriteLine(model.SelectedProductName);
            Console.WriteLine(model.SerialNumber);
            return View("~/Plugins/Misc.WarrantySystem/Views/Register.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(WarrantyRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Products = await _warrantyService.GetAllProductsSelectListAsync();
                return View("~/Plugins/Misc.WarrantySystem/Views/Register.cshtml", model);
            }

            await _warrantyService.InsertWarrantyRegistrationAsync(model);

            TempData["SuccessMessage"] = "Warranty submitted successfully!";
            return RedirectToAction("Register");
        }
    }
}