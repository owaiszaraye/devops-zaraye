using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class RateBuilder : ZarayeEntityBuilder<Rate>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Rate.Price)).AsDecimal().NotNullable();
        }
    }
}