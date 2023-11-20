using Zaraye.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Orders
{
    public partial class Contract : BaseEntity,StoreEntity,DefaultColumns, ISoftDeletedEntity, IActiveActivityLogEntity
    {
        public Guid ContractGuid { get; set; }
        public int OrderId { get; set; }
        public string ContractType { get; set; }
        public int SignaturePictureId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
