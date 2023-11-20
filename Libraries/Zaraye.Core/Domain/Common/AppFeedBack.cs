using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Common
{
    public partial class AppFeedBack : BaseEntity,StoreEntity, DefaultColumns
    {
        public string FeedBack { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
        public bool EmailSentToUser { get; set; }
        public string OwnerEmail { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}