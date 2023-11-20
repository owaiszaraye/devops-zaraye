using System;

namespace Zaraye.Core.Domain.Messages
{
    public partial class CampaignEmail : BaseEntity,StoreEntity, DefaultColumns
    {
        public int CampaignId { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
