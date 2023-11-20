using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Gdpr;

namespace Zaraye.Data.Mapping.Builders.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent entity builder
    /// </summary>
    public partial class GdprConsentBuilder : ZarayeEntityBuilder<GdprConsent>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(GdprConsent.Message)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}