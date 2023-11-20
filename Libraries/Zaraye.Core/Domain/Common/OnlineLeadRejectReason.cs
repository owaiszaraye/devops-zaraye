using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Common
{
    public partial class OnlineLeadRejectReason : BaseEntity
    {
        public string Name { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int DeletedById { get; set; }
    }
}
