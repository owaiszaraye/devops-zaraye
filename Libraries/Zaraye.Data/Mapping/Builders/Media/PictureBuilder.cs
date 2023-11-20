﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Media;

namespace Zaraye.Data.Mapping.Builders.Media
{
    /// <summary>
    /// Represents a picture entity builder
    /// </summary>
    public partial class PictureBuilder : ZarayeEntityBuilder<Picture>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Picture.MimeType)).AsString(40).NotNullable()
                .WithColumn(nameof(Picture.SeoFilename)).AsString(300).Nullable();
        }

        #endregion
    }
}