using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Inventory
{
    public partial class InventoryGroupBuilder : ZarayeEntityBuilder<InventoryGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(InventoryGroup.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(InventoryGroup.BrandId)).AsInt32().ForeignKey<Manufacturer>();
        }

        #endregion
    }
}
