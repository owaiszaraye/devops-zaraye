using Zaraye.Core.Domain.Payments;
using System;

namespace Zaraye.Core.Domain.Shipping
{
    public partial class ShipmentDropOffHistory :BaseEntity
    {  
        public int ShipmentId { get; set; }
        public int TransporterId { get; set; }
        public int VehicleId { get; set; }
        public string DropoffLocation { get; set; }
        public decimal DeliveryCost { get; set; }
        public int TransporterTypeId { get; set; }
        public int RouteTypeId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }

        public TransporterType TransporterType
        {
            get => (TransporterType)TransporterTypeId;
            set => TransporterTypeId = (int)value;
        }

        public RouteType RouteType
        {
            get => (RouteType)RouteTypeId;
            set => RouteTypeId = (int)value;
        }

    }
}
