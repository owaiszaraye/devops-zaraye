using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Orders;
using System;

namespace Zaraye.Core.Domain.Inventory
{
    public partial class InventoryInbound : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity, IActiveActivityLogEntity
    {
        //FK
        public int? PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int? ShipmentId { get; set; }
        public int InventoryGroupId { get; set; }

        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductAttributesXml { get; set; }
        public int BrandId { get; set; }
        public int WarehouseId { get; set; }

        public bool WholesaleTax { get; set; }
        public decimal StockQuantity { get; set; }

        public decimal GstRate { get; set; }
        public decimal GstAmount { get; set; }

        public decimal WhtRate { get; set; }
        public decimal WhtAmount { get; set; }

        public decimal WholeSaleTaxRate { get; set; }
        public decimal WholeSaleTaxAmount { get; set; }

        public decimal PurchaseRate { get; set; }
        public decimal TotalPurchaseValue { get; set; }

        public decimal SupplierCommissionReceivablePerBag { get; set; }
        public int SupplierCommissionReceivableUserId { get; set; }

        public decimal SupplierCommissionPayablePerBag { get; set; }
        public int SupplierCommissionPayableUserId { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }

        public int BusinessModelId { get; set; }

        public int StoreId { get; set; }

        public int InventoryInboundStatusId { get; set; }
        public InventoryInboundStatusEnum InventoryInboundStatusEnum
        {
            get => (InventoryInboundStatusEnum)InventoryInboundStatusId;
            set => InventoryInboundStatusId = (int)value;
        }

        public int InventoryTypeId { get; set; }
        public InventoryTypeEnum InventoryTypeEnum
        {
            get => (InventoryTypeEnum)InventoryTypeId;
            set => InventoryTypeId = (int)value;
        }

        public BusinessModelEnum BusinessModelEnum
        {
            get => (BusinessModelEnum)BusinessModelId;
            set => BusinessModelId = (int)value;
        }
    }
}
