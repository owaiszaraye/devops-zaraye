using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Messages;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Messages
{
    public partial class PushNotificationDeviceBuilder : ZarayeEntityBuilder<PushNotificationDevice>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PushNotificationDevice.DeviceId)).AsString(int.MaxValue).NotNullable();
                //.WithColumn(NameCompatibilityManager.GetColumnName(typeof(PushNotificationDevice), nameof(PushNotificationDevice.CustomerId)))
                //    .AsInt32().PrimaryKey().ForeignKey<Customer>();
        }

        #endregion
    }
}