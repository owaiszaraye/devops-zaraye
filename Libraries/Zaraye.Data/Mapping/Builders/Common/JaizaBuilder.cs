using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class JaizaBuilder : ZarayeEntityBuilder<Jaiza>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Jaiza.Prediction)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Jaiza.Recommendation)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}