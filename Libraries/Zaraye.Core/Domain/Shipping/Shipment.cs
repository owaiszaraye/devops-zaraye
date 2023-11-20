using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Shipping;
using System;
using System.Net;

namespace Zaraye.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class Shipment : BaseEntity,StoreEntity, DefaultColumns
    {
        public string CustomShipmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the tracking number of this shipment
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the total weight of this shipment
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public DateTime? ShippedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public DateTime? DeliveryDateUtc { get; set; }
        public int? DeliveryRequestId { get; set; }

        /// <summary>
        /// Gets or sets the ready for pickup date and time
        /// </summary>
        public DateTime? ReadyForPickupDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        //custom fields
        public string LocalTransporterName { get; set; }
        public string LocalTransporterPhoneNumber { get; set; }
        public int TransporterId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public int DeliveryStatusId { get; set; }
        public string PickupAddress { get; set; }
        public int RouteTypeId { get; set; }
        public string ShipmentDeliveryAddress { get; set; }
        public int WarehouseId { get; set; }
        public int PictureId { get; set; }
        public int TransporterTypeId { get; set; }
        public int DeliveryTypeId { get; set; }
        public int DeliveryTimingId { get; set; }
        public int DeliveryCostTypeId { get; set; }
        public int DeliveryDelayedReasonId { get; set; }
        public int DeliveryCostReasonId { get; set; }

        //shipment and delivery custom fields
        public decimal DeliveryCost { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal LabourCharges { get; set; }
        public decimal OnLabourCharges { get; set; }

        //Expected Fields
        public DateTime? ExpectedDateShipped { get; set; }
        public DateTime? ExpectedDateDelivered { get; set; }
        public decimal ExpectedDeliveryCost { get; set; }
        public decimal ExpectedQuantity { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public decimal DeliveredAmount { get; set; }
        public int ShipmentTypeId { get; set; }
        public int PaymentStatusId { get; set; }
        public bool IsDirectOrder { get; set; }
        public int? BuyerId { get; set; }
        public string Source { get; set; }
        public int ShipmentReturnTypeId { get; set; }

        public int LabourTypeId { get; set; }

        public int CostOnZarayeId { get; set; }

        public decimal ActualShippedQuantity { get; set; }
        public decimal ActualDeliveredQuantity { get; set; }
        public string ActualShippedQuantityReason { get; set; }
        public string  ActualDeliveredQuantityReason { get; set; }

        public decimal CostBore { get; set; }

        public int StoreId { get; set; }

        public ShipmentReturnType ShipmentReturnType
        {
            get => (ShipmentReturnType)ShipmentReturnTypeId;
            set => ShipmentReturnTypeId = (int)value;
        }

        public DeliveryStatus DeliveryStatus
        {
            get => (DeliveryStatus)DeliveryStatusId;
            set => DeliveryStatusId = (int)value;
        }
        public DeliveryCostType DeliveryCostType
        {
            get => (DeliveryCostType)DeliveryCostTypeId;
            set => DeliveryCostTypeId = (int)value;
        }

        public DeliveryType DeliveryType
        {
            get => (DeliveryType)DeliveryTypeId;
            set => DeliveryTypeId = (int)value;
        }

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

        public DeliveryTiming DeliveryTimingEnum
        {
            get => (DeliveryTiming)DeliveryTimingId;
            set => DeliveryTimingId = (int)value;
        }

        public ShipmentType ShipmentType
        {
            get => (ShipmentType)ShipmentTypeId;
            set => ShipmentTypeId = (int)value;
        }
        
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }

        public LabourType LabourType
        {
            get => (LabourType)LabourTypeId;
            set => LabourTypeId = (int)value;
        }

        public CostOnZaraye CostOnZaraye
        {
            get => (CostOnZaraye)CostOnZarayeId;
            set => CostOnZarayeId = (int)value;
        }
    }
}