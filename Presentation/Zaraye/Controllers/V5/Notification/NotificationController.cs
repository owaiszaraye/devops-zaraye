using Azure;
using ExCSS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zaraye.Core;
using Zaraye.Core.Domain.Messages;
using Zaraye.Services.Customers;
using Zaraye.Services.Helpers;
using Zaraye.Services.Messages;
using Zaraye.Models.Api.V5.Notification;

namespace Zaraye.Controllers.V5.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("5")]
    [Route("v{version:apiVersion}/notification")]
    public class NotificationController : BaseApiController
    {
        #region Fields

        private readonly IPushNotificationService _pushNotificationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;


        #endregion

        #region Ctor

        public NotificationController(
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

        [HttpGet("SendDummy")]
        public async Task<IActionResult> senddummy()
        {
           var result = await _pushNotificationService.SendPushNotificationAsync(AppType.ConsumerApp, "test", "test", new string[] { });

            return Ok(result);
        }

        [HttpPost("add-device")]
        public async Task<IActionResult> AddDevice([FromBody] NotificationApiModel.NotificationDeviceApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();

            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            if (string.IsNullOrWhiteSpace(model.Token))
                return Ok(new { success = false, message = "Token is required" });

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

            return Ok(new { success = true, message = "" });
        }

        [HttpPost("remove-device")]
        public async Task<IActionResult> RemoveDevice([FromBody] NotificationApiModel.NotificationDeviceApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            if (string.IsNullOrWhiteSpace(model.Token))
                return Ok(new { success = false, message = "Token is required" });

            var device = await _pushNotificationService.FindPushNotificationDevice(model.Token);
            if (device != null)
            {
                await _pushNotificationService.DeletePushNotificationDeviceAsync(device);
            }

            return Ok(new { success = true, message = "" });
        }

        [HttpGet("all-devices")]
        public async Task<IActionResult> AllDevices(int[] customerRoleIds = null)
        {
            try
            {
                var data = (await _pushNotificationService.GetAllPushNotificationDevicesAsync(customerRoleIds: customerRoleIds)).Select(async c =>
                {
                    return new
                    {
                        Token = c.DeviceId,
                        User = await _customerService.GetCustomerFullNameAsync(c.CustomerId)
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("send-notification-all")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationApiModel.NotificationSendApiModel model)
        {
            try
            {
                if (!model.Token.Any())
                    return Ok(new { success = false, message = "Token is required" });

                if (string.IsNullOrWhiteSpace(model.Title))
                    return Ok(new { success = false, message = "Title is required" });

                if (string.IsNullOrWhiteSpace(model.Body))
                    return Ok(new { success = false, message = "Body is required" });

                //string[] registrationTokens = model.Token;
                //var message = new MulticastMessage()
                //{
                //    Tokens = registrationTokens,
                //    Notification = new Notification()
                //    {
                //        Title = model.Title,
                //        Body = model.Body
                //    },
                //    Data = new Dictionary<string, string>()
                //    {
                //        {"extraId", "2233"}
                //    },
                //    Android = new AndroidConfig()
                //    {
                //        Priority = FirebaseAdmin.Messaging.Priority.High,
                //        Notification = new AndroidNotification()
                //        {
                //            ChannelId = "500"
                //        }
                //    }
                //};

                //var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                //return Ok(new { success = true, message = response });
                return Ok(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-notifications")]
        public async Task<IActionResult> AllNotifications()
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var data = (await _pushNotificationService.GetAllPushNotificationsAsync(customerId: customer.Id)).Select(async c =>
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

                return Ok(new { success = true, data, Count = data.Where(x => !x.IsRead).Count() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("read-notification/{Id}")]
        public async Task<IActionResult> ReadNotification([FromBody] NotificationApiModel.NotificationReadApiModel model)
        {
            try
            {
                var pushNotification = await _pushNotificationService.GetPushNotificationIdAsync(model.Id);
                if (pushNotification is null)
                    return Ok(new { success = false, message = "Invalid id" });

                pushNotification.IsRead = true;
                await _pushNotificationService.UpdatePushNotificationAsync(pushNotification);

                var data = await _pushNotificationService.GetAllPushNotificationsAsync(customerId: pushNotification.CustomerId);
                return Ok(new { success = true, Count = data.Where(x => !x.IsRead).Count() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
