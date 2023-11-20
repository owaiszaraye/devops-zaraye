using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class Rate : BaseEntity,StoreEntity, ISoftDeletedEntity,DefaultColumns, IActiveActivityLogEntity
    {
        public int RateGroupId { get; set; }
        public decimal Price { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}