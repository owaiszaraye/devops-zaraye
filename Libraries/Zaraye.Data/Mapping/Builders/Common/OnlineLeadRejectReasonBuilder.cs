using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class OnlineLeadRejectReasonBuilder : ZarayeEntityBuilder<OnlineLeadRejectReason>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OnlineLeadRejectReason.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}
