using Zaraye.Models.Api.V4.Notification;

namespace Zaraye.Services.Notification
{
    public interface INotificationControllerService
    {
        Task<object> AddDevice(NotificationApiModel.NotificationDeviceApiModel model);
        Task<object> RemoveDevice(NotificationApiModel.NotificationDeviceApiModel model);
        Task<object> GetAllDevices(int[] customerRoleIds = null);
        Task<object> SendNotification(NotificationApiModel.NotificationSendApiModel model);
        Task<IList<object>> GetAllNotifications();
        Task<object> ReadNotification(NotificationApiModel.NotificationReadApiModel model);
    }
}
