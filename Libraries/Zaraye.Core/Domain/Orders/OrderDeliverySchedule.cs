using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    public class OrderDeliverySchedule: BaseEntity,StoreEntity, IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        public int OrderId { get; set; }
        public DateTime ExpectedShipmentDateUtc { get; set; }
        public DateTime ExpectedDeliveryDateUtc { get; set; }
        public decimal ExpectedQuantity { get; set; }
        public decimal ExpectedDeliveryCost { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }

        public int StoreId { get; set; }
    }
}
