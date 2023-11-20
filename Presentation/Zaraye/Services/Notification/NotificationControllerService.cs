using Zaraye.Core;
using Newtonsoft.Json;
using Zaraye.Core.Domain.Messages;
using Zaraye.Services.Customers;
using Zaraye.Services.Helpers;
using Zaraye.Services.Messages;
using Zaraye.Models.Api.V4.Notification;


namespace Zaraye.Services.Notification
{
    public class NotificationControllerService : INotificationControllerService
    {
        #region Fields

        private readonly IPushNotificationService _pushNotificationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor
        public NotificationControllerService(
            IPushNotificationService pushNotificationService,
            ICustomerService customerService,
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper)
        {
            _pushNotificationService = pushNotificationService;
            _customerService = customerService;
            _workContext = workContext;
            _dateTimeHelper = dateTimeHelper;
        }
        #endregion

        #region Methods
        public virtual async Task<object> AddDevice(NotificationApiModel.NotificationDeviceApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();

            if (string.IsNullOrWhiteSpace(model.Token))
                throw new ApplicationException("Token is required");

            var device = await _pushNotificationService.FindPushNotificationDevice(model.Token);
            if (device == null)
            {
                var newDevice = new PushNotificationDevice
                {
                    CustomerId = user.Id,
                    DeviceId = model.Token,
                    CreatedOnUtc = DateTime.UtcNow,
                    Active = true
                };
                await _pushNotificationService.InsertPushNotificationDeviceAsync(newDevice);
            }
            else
            {
                device.CustomerId = user.Id;
                device.DeviceId = model.Token;
                await _pushNotificationService.UpdatePushNotificationDeviceAsync(device);
            }

            return "";
        }
        public virtual async Task<object> RemoveDevice(NotificationApiModel.NotificationDeviceApiModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
                throw new ApplicationException("Token is required");

            var device = await _pushNotificationService.FindPushNotificationDevice(model.Token);
            if (device != null)
            {
                await _pushNotificationService.DeletePushNotificationDeviceAsync(device);
            }
            return "";
        }
        public virtual async Task<object> GetAllDevices(int[] customerRoleIds = null)
        {
            var data = (await _pushNotificationService.GetAllPushNotificationDevicesAsync(customerRoleIds: customerRoleIds)).Select(async c =>
            {
                return new
                {
                    Token = c.DeviceId,
                    User = await _customerService.GetCustomerFullNameAsync(c.CustomerId)
                };
            }).Select(t => t.Result).ToList();

            return data;
        }
        public virtual async Task<object> SendNotification(NotificationApiModel.NotificationSendApiModel model)
        {
            if (!model.Token.Any())
                throw new ApplicationException("Token is required");

            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ApplicationException("Title is required");

            if (string.IsNullOrWhiteSpace(model.Body))
                throw new ApplicationException("Body is required");

            return "";
        }
        public virtual async Task<IList<object>> GetAllNotifications()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            List<object> result = new List<object>();
            var notifications = (await _pushNotificationService.GetAllPushNotificationsAsync(customerId: customer.Id)).Select(async c =>
            {
                return new
                {
                    Id = c.Id,
                    Title = c.Title,
                    Body = c.Body,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(c.CreatedOnUtc, DateTimeKind.Utc),
                    IsRead = c.IsRead,
                    ExtraData = !string.IsNullOrWhiteSpace(c.ExtraData) ? JsonConvert.DeserializeObject(c.ExtraData) : "{}"
                };
            }).Select(t => t.Result).ToList();

            result.Add(notifications);
            result.Add(notifications.Where(x => !x.IsRead).Count());

            return result;
        }
        public virtual async Task<object> ReadNotification(NotificationApiModel.NotificationReadApiModel model)
        {
            var pushNotification = await _pushNotificationService.GetPushNotificationIdAsync(model.Id);
            if (pushNotification is null)
                throw new ApplicationException("Invalid id");

            pushNotification.IsRead = true;
            await _pushNotificationService.UpdatePushNotificationAsync(pushNotification);

            var data = await _pushNotificationService.GetAllPushNotificationsAsync(customerId: pushNotification.CustomerId);

            return data.Where(x => !x.IsRead).Count();
        }
        #endregion
    }
}
