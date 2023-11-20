using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class WebSliderBuilder : ZarayeEntityBuilder<WebSlider>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WebSlider.Title)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}
