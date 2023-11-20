using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.Customers
{
    public partial class TransporterCostBuilder : ZarayeEntityBuilder<TransporterCost>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TransporterCost.VehicleTransporterMappingId)).AsInt32().Nullable();
        }

        #endregion
    }
}
