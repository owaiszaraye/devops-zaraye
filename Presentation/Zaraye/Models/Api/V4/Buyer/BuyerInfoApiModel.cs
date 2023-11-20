using Newtonsoft.Json;

namespace Zaraye.Models.Api.V4.Buyer
{
    public partial class BuyerInfoApiModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string Phone { get; set; }
        public int IndustryId { get; set; }
        public int BuyerTypeId { get; set; }
        public int AreaId { get; set; }
        public BuyerPinLocationApiModel? BuyerPinLocation { get; set; }
        public class BuyerPinLocationApiModel
        {
            [JsonProperty("latitude")]
            public string Latitude { get; set; }

            [JsonProperty("longitude")]
            public string Longitude { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }
        }
    }

    public partial class BuyerRequestApiModel
    {
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public string OtherBrand { get; set; }
        public decimal Quantity { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int PaymentDuration { get; set; }
        public decimal IdealBuyingPrice { get; set; }

        public List<AttributesApiModel> AttributesData { get; set; }

        public class AttributesApiModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
    
    public partial class BuyerRequestBidApproveApiModel
    {
        public int BuyerRequestId { get; set; }
        public List<int> QuotationId { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryAddress2 { get; set; }
        public BuyerRequestPinLocationApiModel PinLocation { get; set; }
    }

    public partial class Booker_BuyerRequestBidApproveApiModel
    {
        public int BuyerRequestId { get; set; }
        public List<int> QuotationId { get; set; }

    }

    public partial class BookerBuyerRequestApiModel
    {
        public int BuyerId { get; set; }
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string OtherBrand { get; set; }
        public int BrandId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int PaymentDuration { get; set; }

        public string DeliveryAddress { get; set; }
        public string DeliveryAddress2 { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }

        public List<BookerAttributesApiModel> AttributesData { get; set; }
        public BuyerRequestPinLocationApiModel PinLocation { get; set; }

        public class BookerAttributesApiModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }

    public class BuyerRequestPinLocationApiModel
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Location { get; set; }
    }

    public class BuyerInfoAddressApiModel
    {
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int AreaId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public BuyerInfoAddressPinLocationApiModel BuyerPinLocation { get; set; }
        public class BuyerInfoAddressPinLocationApiModel
        {
            [JsonProperty("latitude")]
            public string Latitude { get; set; }

            [JsonProperty("longitude")]
            public string Longitude { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }
        }
    }

    public class BuyerOrderPaymentApiModel
    {
        public string CustomOrderNumber { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
        public string DateFormatted { get; set; }
        public DateTime Date { get; set; }
        public bool IsUp { get; set; }
    }

    public class BuyerContractUploadSignatureModel
    {
        public int OrderId { get; set; }
        public int ContactId { get; set; }
        public byte[] imgBytes { get; set; }
    }


    public class ChangeBuyerPasswordApiModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public partial class BuyerLedgerDetailApiModel
    {
        public BuyerLedgerDetailApiModel()
        {
            buyerLedgerlistModel = new List<BuyerLedgerDeatailListApiModel>();
        }
        public string date { get; set; }
        public IList<BuyerLedgerDeatailListApiModel> buyerLedgerlistModel { get; set; }
    }
    public class BuyerLedgerDeatailListApiModel
    {
        public BuyerLedgerDeatailListApiModel()
        {
            PaymentDetails = new List<LedgerPaymentDetailModel>();
        }

        public string deliveredDate { get; set; }
        public string description { get; set; }
        public string balance { get; set; }
        public string outstandingBalance { get; set; }
        public int orderId { get; set; }
        public string customOrderNumber { get; set; }
        public decimal quantity { get; set; }
        public string quantityType { get; set; }
        public string debitFormatted { get; set; }
        public decimal debit { get; set; }
        public string creditFormatted { get; set; }
        public decimal credit { get; set; }
        public string sku { get; set; }
        public int shipmentId { get; set; }
        public string customShipmentNumber { get; set; }
        public int brandId { get; set; }
        public string brand { get; set; }
        public int paymentId { get; set; }
        public IList<LedgerPaymentDetailModel> PaymentDetails { get; set; }

        public class LedgerPaymentDetailModel
        {
            public int ShipmentId { get; set; }
            public string CustomShipmentNumber { get; set; }
            public int OrderId { get; set; }
            public string CustomOrderNumber { get; set; }
            public string AmountFormatted { get; set; }
            public string DateFormatted { get; set; }
        }
    }
}