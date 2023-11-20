using System;

namespace Zaraye.Core.Domain.Messages
{
    public partial class PushNotification : BaseEntity
    {
        public int CustomerId { get; set; }
        
        public int? EntityId { get; set; }
        public string EntityName { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime? DontSendBeforeDateUtc { get; set; }

        public bool IsRead { get; set; }

        public string ExtraData { get; set; }
    }
}
