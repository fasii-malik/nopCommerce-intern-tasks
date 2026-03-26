using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record ImportResultModel : BaseNopModel
    {
        public ImportResultModel()
        {
            SuccessMessages = new List<string>();
            ErrorMessages = new List<string>();
        }

        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> SuccessMessages { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}