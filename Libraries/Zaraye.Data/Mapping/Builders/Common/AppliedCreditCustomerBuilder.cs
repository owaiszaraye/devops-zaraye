using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class AppliedCreditCustomerBuilder : ZarayeEntityBuilder<AppliedCreditCustomer>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //table
            //    .WithColumn(nameof(AppliedCreditCustomer.Cnic)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}