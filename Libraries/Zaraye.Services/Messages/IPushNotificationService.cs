using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Messages;

namespace Zaraye.Services.Messages
{
    public partial interface IPushNotificationService
    {
        #region Methods

        #region PushNotificationDevice Methods

        Task InsertPushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice);

        Task UpdatePushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice);

        Task DeletePushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice);

        Task<PushNotificationDevice> GetPushNotificationDeviceByIdAsync(int pushNotificationDeviceId);

        Task<IList<PushNotificationDevice>> GetAllPushNotificationDevicesAsync(int customerId = 0, int[] customerRoleIds = null);

        Task<PushNotificationDevice> FindPushNotificationDevice(string deviceId);

        #endregion

        #region PushNotification Methods

        Task<PushNotification> InsertPushNotificationAsync(int customerId, string title, string body, int? entityId, string entity = null, string extraData = null);

        Task UpdatePushNotificationAsync(PushNotification pushNotification);

        Task<PushNotification> GetPushNotificationIdAsync(int pushNotificationId);

        Task<IPagedList<PushNotification>> GetAllPushNotificationsAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
        int? customerId = null, string entityName = null, int? entityId = null,
        int pageIndex = 0, int pageSize = int.MaxValue);

        Task<PushNotificationresponse> SendPushNotificationAsync(AppType appType, string title, string body, string[] token);

        #endregion

        #endregion
    }
}
