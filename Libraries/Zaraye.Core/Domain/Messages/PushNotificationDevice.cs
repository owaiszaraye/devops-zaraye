using System;

namespace Zaraye.Core.Domain.Messages
{
    public partial class PushNotificationDevice : BaseEntity
    {
        public int CustomerId { get; set; }
        public string DeviceId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Active { get; set; }
    }
}
