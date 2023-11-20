using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order item entity builder
    /// </summary>
    public partial class RequestForQuotationBuilder : ZarayeEntityBuilder<RequestForQuotation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RequestForQuotation.RequestId)).AsInt32().ForeignKey<Request>();
        }

        #endregion
    }
}