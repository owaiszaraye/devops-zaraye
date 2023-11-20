using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    public partial record ProductTagModel : BaseZarayeEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int ProductCount { get; set; }
    }
}