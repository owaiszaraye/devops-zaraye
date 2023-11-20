using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Configuration;

namespace Zaraye.Data.Mapping.Builders.Configuration
{
    public partial class BankAccountBuilder : ZarayeEntityBuilder<BankAccount>
    {
        #region Methods
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {

        }

        #endregion
    }
}
