using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class Request : BaseEntity,StoreEntity, IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        public string CustomRequestNumber { get; set; }
        public int BuyerId { get; set; }
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductAttributeXml { get; set; }
        public int BrandId { get; set; }
        public decimal Quantity { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryAddress2 { get; set; }
        public DateTime DeliveryDate { get; set; }

        public int RequestStatusId { get; set; }
        public int BookerId { get; set; }
        public int PaymentDuration { get; set; }
        public string RejectedReason { get; set; }

        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string PinLocation_Latitude { get; set; }
        public string PinLocation_Longitude { get; set; }
        public string PinLocation_Location { get; set; }
        public int BusinessModelId { get; set; }
        public bool InterGeography { get; set; }
        public decimal IdealBuyingPrice { get; set; }
        public int RequestTypeId { get; set; }
        //public string OtherBrand { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public string Source { get; set; }
        public int LeadId { get; set; }
        public int PocId { get; set; }

        public int StoreId { get; set; }

        public RequestStatus RequestStatus
        {
            get => (RequestStatus)RequestStatusId;
            set => RequestStatusId = (int)value;
        }

        public BusinessModelEnum BusinessModelEnum
        {
            get => (BusinessModelEnum)BusinessModelId;
            set => BusinessModelId = (int)value;
        }

        public RequestTypeEnum RequestTypeEnum
        {
            get => (RequestTypeEnum)RequestTypeId;
            set => RequestTypeId = (int)value;
        }
    }
}
