using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Media;

namespace Zaraye.Data.Mapping.Builders.Media
{
    /// <summary>
    /// Represents a video entity builder
    /// </summary>
    public partial class VideoBuilder : ZarayeEntityBuilder<Video>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Video.VideoUrl)).AsString(1000).NotNullable();
        }

        #endregion
    }
}
