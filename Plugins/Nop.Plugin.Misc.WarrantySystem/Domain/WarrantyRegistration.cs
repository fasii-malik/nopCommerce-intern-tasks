using System;
using Nop.Core;

namespace Nop.Plugin.Misc.WarrantySystem.Domain;

public class WarrantyRegistration : BaseEntity
{
    public int CustomerId { get; set; }

    public string ProductName { get; set; }

    public string SerialNumber { get; set; }

    public int? InvoicePictureId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}