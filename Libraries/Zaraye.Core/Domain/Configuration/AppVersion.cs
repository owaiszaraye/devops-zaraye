using Zaraye.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Configuration
{
    public class AppVersion : BaseEntity,StoreEntity, ISoftDeletedEntity, DefaultColumns
    {
        public string Type { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Comments { get; set; }
        public bool ForceUpdate { get; set; }
        public bool Published { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
