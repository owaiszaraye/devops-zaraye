using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.Configuration
{
    public partial class AppVersionBuilder : ZarayeEntityBuilder<AppVersion>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(AppVersion.Version)).AsString(500).NotNullable();
        }
    }
}
