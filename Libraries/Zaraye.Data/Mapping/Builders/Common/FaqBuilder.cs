using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class FaqBuilder : ZarayeEntityBuilder<Faq>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Faq.Question)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Faq.Answer)).AsString(int.MaxValue).NotNullable();
                        }

        #endregion
    }
}