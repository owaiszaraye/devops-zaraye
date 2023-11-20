using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Inventory
{
    public partial class InventoryOutbound : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity
    {
        //FK
        public int InventoryInboundId { get; set; }
        public int SaleOrderId { get; set; }
        public int InventoryGroupId { get; set; }
        public int OrderItemId { get; set; }
        public int ShipmentId { get; set; }
        public bool Deleted { get; set; }
        public decimal OutboundQuantity { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public int DeletedById { get; set; }

        public int StoreId { get; set; }

        public int InventoryOutboundStatusId { get; set; }
        public InventoryOutboundStatusEnum InventoryOutboundStatusEnum
        {
            get => (InventoryOutboundStatusEnum)InventoryOutboundStatusId;
            set => InventoryOutboundStatusId = (int)value;
        }
        public int InventoryOutboundTypeId { get; set; }
        public InventoryOutboundTypeEnum InventoryOutboundTypeEnum
        {
            get => (InventoryOutboundTypeEnum)InventoryOutboundTypeId;
            set => InventoryOutboundTypeId = (int)value;
        }
    }
}
