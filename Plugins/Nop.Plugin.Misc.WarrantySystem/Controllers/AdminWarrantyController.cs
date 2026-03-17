using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.WarrantySystem.Services;

namespace Nop.Plugin.Misc.WarrantySystem.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class AdminWarrantyController : BasePluginController
    {
        private readonly IWarrantyService _warrantyService;

        public AdminWarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService;
        }

        public IActionResult List()
        {
            var model = _warrantyService.GetAllWarranties();
            return View("~/Plugins/Misc.WarrantySystem/Views/Admin/List.cshtml", model);
        }

        public IActionResult Approve(int id)
        {
            _warrantyService.UpdateStatus(id, 1); // 1 = Approved
            return RedirectToAction("List");
        }

        public IActionResult Reject(int id)
        {
            _warrantyService.UpdateStatus(id, 2); // 2 = Rejected
            return RedirectToAction("List");
        }
    }
}