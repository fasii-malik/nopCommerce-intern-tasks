using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.WarrantySystem.Domain;
using Nop.Plugin.Misc.WarrantySystem.Models;

namespace Nop.Plugin.Misc.WarrantySystem.Services
{
    public interface IWarrantyService
    {
        Task InsertWarrantyRegistrationAsync(WarrantyRegisterModel model);
        IList<SelectListItem> GetAllProductsSelectList();
        Task<IList<SelectListItem>> GetAllProductsSelectListAsync();
        IList<WarrantyRegistration> GetAllWarranties();
        void UpdateStatus(int id, int status);
    }
}