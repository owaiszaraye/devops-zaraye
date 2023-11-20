using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class FavouriteRateGroupBuilder : ZarayeEntityBuilder<FavouriteRateGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(FavouriteRateGroup.CustomerId)).AsInt32().ForeignKey<Customer>();
        }

        #endregion
    }
}