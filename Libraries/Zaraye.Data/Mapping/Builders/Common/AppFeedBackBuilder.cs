using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class AppFeedBackBuilder : ZarayeEntityBuilder<AppFeedBack>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AppFeedBack.FeedBack)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(AppFeedBack.Rating)).AsString(int.MaxValue).NotNullable();
                        }

        #endregion
    }
}