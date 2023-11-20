using Zaraye.Core.Domain.Buyer;
using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class OnlineLead : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity, IActiveActivityLogEntity
    {
        public string Service { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        
        public int StatusId { get; set; }
        public string Source { get; set; }

        public int CountryId { get; set; }
        public int CityId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public string Email { get; set; }
        public string CityName { get; set; }

        public int CustomerId { get; set; }
        public string Comment { get; set; }

        public int ReasonId { get; set; }

        public int StoreId { get; set; }
       // public bool IncludeInTopMenu { get; set; }

        public OnlineLeadStatus OnlineLeadStatus
        {
            get => (OnlineLeadStatus)StatusId;
            set => StatusId = (int)value;
        }
    }
}