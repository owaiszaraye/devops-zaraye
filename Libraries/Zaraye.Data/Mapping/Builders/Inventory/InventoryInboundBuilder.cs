using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;
using System.Data;

namespace Zaraye.Data.Mapping.Builders.Inventory
{
    public partial class InventoryInboundBuilder : ZarayeEntityBuilder<InventoryInbound>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(InventoryInbound.SupplierId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(InventoryInbound), nameof(InventoryInbound.ShipmentId))).AsInt32().ForeignKey<Shipment>(onDelete: Rule.None).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(InventoryInbound), nameof(InventoryInbound.PurchaseOrderId))).AsInt32().ForeignKey<Order>(onDelete: Rule.None).Nullable()
                .WithColumn(nameof(InventoryInbound.IndustryId)).AsInt32().ForeignKey<Industry>()
                .WithColumn(nameof(InventoryInbound.CategoryId)).AsInt32().ForeignKey<Category>()
                .WithColumn(nameof(InventoryInbound.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(InventoryInbound.WarehouseId)).AsInt32().ForeignKey<Warehouse>()
                .WithColumn(nameof(InventoryInbound.BrandId)).AsInt32().ForeignKey<Manufacturer>();
        }

        #endregion
    }
}
