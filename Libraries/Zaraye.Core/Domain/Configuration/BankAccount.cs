using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Configuration
{
    public partial class BankAccount : BaseEntity,StoreEntity, DefaultColumns, IActiveActivityLogEntity, ISoftDeletedEntity
    {
        public string BankName { get; set; }
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public bool Active { get; set; }

        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
