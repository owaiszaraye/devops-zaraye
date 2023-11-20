using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    public class OrderDeliveryRequest : BaseEntity,StoreEntity, IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        public int OrderId { get; set; }
        public int OrderDeliveryScheduleId { get; set; }

        public int StatusId { get; set; }       
        public bool BagsDirectlyFromSupplier { get; set; }
        public bool BagsDirectlyFromWarehouse { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public int WarehouseId { get; set; }
        public string StreetAddress { get; set; }
        public string ContactNumber { get; set; }
        public decimal Quantity { get; set; }
        public int AgentId { get; set; }
        public DateTime? TicketExpiryDate { get; set; }
        public int TicketPirority { get; set; }
        public string RejectedReason { get; set; }
        public int VerifiedUserId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }

        public int StoreId { get; set; }

        //Not For App Screen

        //public DateTime ShipmentDateUtc { get; set; }
        //public DateTime? DeliveryDate { get; set; }
        //public int DocumentId { get; set; }
        //public string Notes { get; set; }
        //public string AlternatePickupAddress { get; set; }
        //public decimal TotalDeliveryCost { get; set; }
        //public string Kilometers { get; set; }
        //public string TransporterName { get; set; }

        public OrderDeliveryRequestEnum OrderDeliveryRequestEnum
        {
            get => (OrderDeliveryRequestEnum)StatusId;
            set => StatusId = (int)value;
        }
        public TicketEnum TicketEnum
        {
            get => (TicketEnum)TicketPirority;
            set => TicketPirority = (int)value;
        }

    }
}
