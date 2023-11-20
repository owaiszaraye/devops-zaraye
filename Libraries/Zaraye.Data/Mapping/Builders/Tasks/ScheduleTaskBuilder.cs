using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.ScheduleTasks;

namespace Zaraye.Data.Mapping.Builders.Tasks
{
    /// <summary>
    /// Represents a schedule task entity builder
    /// </summary>
    public partial class ScheduleTaskBuilder : ZarayeEntityBuilder<ScheduleTask>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScheduleTask.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ScheduleTask.Type)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}