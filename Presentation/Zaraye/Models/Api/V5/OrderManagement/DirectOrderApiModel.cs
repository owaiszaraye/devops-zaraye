using System;
using System.Collections.Generic;

namespace Zaraye.Models.Api.V5.OrderManagement
{
    public partial class DirectOrderApiModel
    {
        public DirectOrderInfoModel DirectOrderInfo { get; set; }
        //public DirectOrderDeliveryInfoModel DeliveryInfo { get; set; }
        //public DirectOrderPaymentInfoModel PaymentInfo { get; set; }
        public partial class DirectOrderInfoModel
        {
            public DirectOrderInfoModel()
            {
                DeliverySchedule = new List<DirectOrderDeliveryScheduleModel>();
            }
            public int RequestId { get; set; }
            public int? CountryId { get; set; }
            public int? CityId { get; set; }
            public int? AreaId { get; set; }
            public string StreetAddress { get; set; }
            public bool? InterGeography { get; set; }
            public int? TransactionModelId { get; set; }
            public DirectOrderPinLocationModel PinLocation { get; set; }
            public IList<DirectOrderDeliveryScheduleModel> DeliverySchedule { get; set; }
        }
        //public partial class DirectOrderDeliveryInfoModel
        //{
        //    public DirectOrderDeliveryInfoModel()
        //    {
        //        DeliverySchedule = new List<DirectOrderDeliveryScheduleModel>();
        //    }
        //    public int CountryId { get; set; }
        //    public int CityId { get; set; }
        //    public int AreaId { get; set; }
        //    public string? StreetAddress { get; set; }
        //    public DirectOrderPinLocationModel PinLocation { get; set; }
        //    public bool? InterGeography { get; set; }
        //    public bool? Step2 { get; set; }
        //    public IList<DirectOrderDeliveryScheduleModel> DeliverySchedule { get; set; }
        //}
        //public partial class DirectOrderPaymentInfoModel
        //{
        //    public DirectOrderPaymentInfoModel()
        //    {
        //        SupplierInfo = new List<SupplierInfoModel>();
        //    }
        //    public int? TransactionModelId { get; set; }
        //    public string TransactionModelName { get; set; }
        //    public int? BuyerPaymentTerm { get; set; }
        //    public bool? Step3 { get; set; }
        //    public IList<SupplierInfoModel> SupplierInfo { get; set; }
        //}
        public class DirectOrderPinLocationModel
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Location { get; set; }
        }
        public class DirectOrderDeliveryScheduleModel
        {
            public int Id { get; set; }
            public int tempOrderId { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public DateTime? ShipmentDate { get; set; }
            public decimal? DeliveryCost { get; set; }
            public decimal? Quantity { get; set; }
        }
        public class SupplierInfoModel
        {
            public int Id { get; set; }
            public int SupplierId { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class DirectOrderAttributesModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class DirectOrderProductItemAttributeApiModel
        {
            public int AttributeControlTypeId { get; set; }
            public int AttributeId { get; set; }
            public string AttributeName { get; set; }
            public int ValueId { get; set; }
            public string Value { get; set; }
        }

        public class DirectCogsInventoryTaggingModel
        {
            public int requestId { get; set; }
            public int inventoryId { get; set; }
            public decimal quantity { get; set; }
        }
    }
}