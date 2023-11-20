using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class OnlineLeadBuilder : ZarayeEntityBuilder<OnlineLead>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OnlineLead.Name)).AsString(400).NotNullable();
        }
    }
}