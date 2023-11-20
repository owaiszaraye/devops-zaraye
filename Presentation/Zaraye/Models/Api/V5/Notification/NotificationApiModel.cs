using Zaraye.Models.Api.V5.Common;

namespace Zaraye.Models.Api.V5.Notification
{
    public partial class NotificationApiModel : BaseApiModel
    {
        public class NotificationDeviceApiModel
        {
            public string Token { get; set; }
        }

        public class NotificationSendApiModel
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string [] Token { get; set; }
        }

        public class NotificationReadApiModel
        {
            public int Id { get; set; }
        }
    }
}