using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a shopping cart item entity builder
    /// </summary>
    public partial class ShoppingCartItemBuilder : ZarayeEntityBuilder<ShoppingCartItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShoppingCartItem.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(ShoppingCartItem.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}