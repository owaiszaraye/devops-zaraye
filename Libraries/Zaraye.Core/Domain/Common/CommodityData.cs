using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Localization;

namespace Zaraye.Core.Domain.Common
{
    public partial class CommodityData : BaseEntity, ILocalizedEntity, ISoftDeletedEntity, DefaultColumns
    {
        public string Name { get; set; }
        public string Percentage { get; set; }
        public string Rate { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
