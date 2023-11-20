using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    
    public partial record ManufacturerBriefInfoModel : BaseZarayeEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        
        public bool IsActive { get; set; }
    }
}