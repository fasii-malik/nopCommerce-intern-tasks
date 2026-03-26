using CsvHelper.Configuration.Attributes;

namespace Nop.Web.Models.Catalog
{
    public class ProductCsvModel
    {
        [Name("Name")]
        public string Name { get; set; }

        [Name("Price")]
        public decimal Price { get; set; }

        [Name("Category")]
        public string Category { get; set; }

        [Name("Stock")]
        public int Stock { get; set; }
    }
}