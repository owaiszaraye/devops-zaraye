using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core;

namespace Zaraye.Core.Domain.EmployeeInsights
{
    public partial class EmployeeInsights : BaseEntity, ILocalizedEntity, DefaultColumns, ISoftDeletedEntity
    {
        public int PictureId { get; set; }
        public string Designation { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DisplayOrder { get; set; }
        public bool Published { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
