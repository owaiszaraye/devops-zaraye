using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Data.Mapping.Builders.Common
{
    public partial class AwsS3FilesBuilder : ZarayeEntityBuilder<AwsS3Files>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AwsS3Files.FileName)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(AwsS3Files.FileType)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}
