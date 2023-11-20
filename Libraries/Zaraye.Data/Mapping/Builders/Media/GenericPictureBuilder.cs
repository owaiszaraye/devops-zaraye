using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.Media
{
    public partial class GenericPictureBuilder : ZarayeEntityBuilder<GenericPicture>
    {
        #region Methods
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GenericPicture.EntityType)).AsInt32();
        }

        #endregion
    }
}
