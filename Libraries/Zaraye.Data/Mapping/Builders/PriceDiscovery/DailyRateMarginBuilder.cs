using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.PriceDiscovery;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.PriceDiscovery
{
    public partial class DailyRateMarginBuilder : ZarayeEntityBuilder<DailyRateMargin>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DailyRateMargin.CityId)).AsInt32().NotNullable()
                .WithColumn(nameof(DailyRateMargin.DailyRateId)).AsInt32().ForeignKey<DailyRate>();
        }

        #endregion
    }
}
