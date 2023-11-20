using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.Customers
{
    public partial class BankDetailBuilder : ZarayeEntityBuilder<BankDetail>
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
