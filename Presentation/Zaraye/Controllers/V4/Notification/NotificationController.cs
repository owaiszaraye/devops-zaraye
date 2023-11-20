using Microsoft.AspNetCore.Mvc;
using Zaraye.Models.Api.V4.Notification;
using Zaraye.Services.Notification;
using Zaraye.Services.Logging;

namespace Zaraye.Controllers.V4.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/notification")]
    public class NotificationController : BaseApiController
    {
        #region Fields

        private readonly INotificationControllerService _notificationService;
        private readonly IAppLoggerService _appLoggerService;

        #endregion

        #region Ctor

        public NotificationController(INotificationControllerService notificationService, IAppLoggerService appLoggerService)
        {
            _notificationService = notificationService;
            _appLoggerService = appLoggerService;
        }

        #endregion

        [HttpPost("add-device")]
        public async Task<IActionResult> AddDevice([FromBody] NotificationApiModel.NotificationDeviceApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var data = await _notificationService.AddDevice(model);

                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("remove-device")]
        public async Task<IActionResult> RemoveDevice([FromBody] NotificationApiModel.NotificationDeviceApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });
                var data = await _notificationService.RemoveDevice(model);

                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-devices")]
        public async Task<IActionResult> AllDevices(int[] customerRoleIds = null)
        {
            try
            {
                var data = await _notificationService.GetAllDevices(customerRoleIds);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("send-notification-all")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationApiModel.NotificationSendApiModel model)
        {
            try
            {
                var data = await _notificationService.SendNotification(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-notifications")]
        public async Task<IActionResult> AllNotifications()
        {
            try
            {
                var data = await _notificationService.GetAllNotifications();

                return Ok(new { success = true, data = data[0], Count = data[1] });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("read-notification/{Id}")]
        public async Task<IActionResult> ReadNotification([FromBody] NotificationApiModel.NotificationReadApiModel model)
        {
            try
            {
                var data = await _notificationService.ReadNotification(model);

                return Ok(new { success = true, Count = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
