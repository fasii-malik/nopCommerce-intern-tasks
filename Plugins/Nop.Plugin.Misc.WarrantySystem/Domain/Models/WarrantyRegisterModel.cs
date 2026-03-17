using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.WarrantySystem.Models
{
    public class WarrantyRegisterModel
    {
        [Display(Name = "ProductId")]
        public int SelectedProductId { get; set; }
        [Display(Name = "Product")]
        public string SelectedProductName { get; set; }  // store product name for now
        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Display(Name = "Invoice (optional)")]
        public IFormFile InvoiceFile { get; set; }

        // Dropdown list for the view
        public IList<SelectListItem> Products { get; set; }

        // Automatically populated from the logged-in customer
        public int CustomerId { get; set; }
    }
}