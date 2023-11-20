using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class IndustryBuilder : ZarayeEntityBuilder<Industry>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Industry.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Industry.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Industry.MetaTitle)).AsString(400).Nullable();
        }
    }
}