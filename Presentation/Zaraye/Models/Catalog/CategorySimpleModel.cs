using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    public partial record CategorySimpleModel : BaseZarayeEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public string IconUrl { get; set; }
        public List<CategorySimpleModel> SubCategories { get; set; }

        public bool HaveSubCategories { get; set; }

        public string Route { get; set; }
    }
}