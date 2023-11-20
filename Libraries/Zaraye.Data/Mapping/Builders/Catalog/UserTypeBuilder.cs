using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class UserTypeBuilder : ZarayeEntityBuilder<UserType>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserType.Name)).AsString(500).NotNullable()
                .WithColumn(nameof(UserType.Type)).AsString(500).NotNullable();
        }

        #endregion
    }
}