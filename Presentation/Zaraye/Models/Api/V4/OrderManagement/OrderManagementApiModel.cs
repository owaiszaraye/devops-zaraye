using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Zaraye.Models.Api.V4.OrderManagement
{
    public partial class OrderManagementApiModel
    {
        public class BuyerInfoModel
        {
            [Required(ErrorMessage = "Email is required")]
            public string Email { get; set; }
            [Required(ErrorMessage = "FirstName is required")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "LastName is required")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Company is required")]
            public string Company { get; set; }
            [Required(ErrorMessage = "Address is required")]
            public string Address { get; set; }
            public string Address2 { get; set; }
            [Required(ErrorMessage = "Country is required")]
            public int CountryId { get; set; }
            [Required(ErrorMessage = "State is required")]
            public int StateId { get; set; }
            [Required(ErrorMessage = "Phone is required")]
            public string Phone { get; set; }
            [Required(ErrorMessage = "Industry is required")]
            public int IndustryId { get; set; }
            [Required(ErrorMessage = "BuyerType is required")]
            public int BuyerTypeId { get; set; }
            [Required(ErrorMessage = "Area is required")]
            public int AreaId { get; set; }
            public BuyerPinLocationModel BuyerPinLocation { get; set; }
            public class BuyerPinLocationModel
            {
                [JsonProperty("latitude")]
                public string Latitude { get; set; }

                [JsonProperty("longitude")]
                public string Longitude { get; set; }

                [JsonProperty("location")]
                public string Location { get; set; }
            }
        }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string Phone { get; set; }
        public int IndustryId { get; set; }
        public int BuyerTypeId { get; set; }
        public int AreaId { get; set; }
        public BuyerPinLocationModel BuyerPinLocation { get; set; }
        public class BuyerPinLocationModel
        {
            [JsonProperty("latitude")]
            public string Latitude { get; set; }

            [JsonProperty("longitude")]
            public string Longitude { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }
        }
        public class BuyerRequestModel
        {
            [Required(ErrorMessage ="BuyerId is required")]
            public int BuyerId { get; set; }
            public string BuyerName { get; set; }
            [Required(ErrorMessage = "IndustryId is required")]
            public int IndustryId { get; set; }
            [Required(ErrorMessage = "CategoryId is required")]
            public int CategoryId { get; set; }
            [Required(ErrorMessage = "ProductId is required")]
            public int ProductId { get; set; }
            [Required(ErrorMessage = "BrandId is required")]
            public int BrandId { get; set; }
            //public string OtherBrand { get; set; }
            public decimal Quantity { get; set; }
            [Required(ErrorMessage = "CountryId is required")]
            public int CountryId { get; set; }
            [Required(ErrorMessage = "CityId is required")]
            public int CityId { get; set; }
            [Required(ErrorMessage = "AreaId is required")]
            public int AreaId { get; set; }
            public string DeliveryAddress { get; set; }
            public DateTime DeliveryDate { get; set; }
            public int TransactionModelId { get; set; }
            //public bool InterGeography { get; set; }
            //public bool GST { get; set; }
            public int PaymentTerms { get; set; }
            //public string Comment { get; set; }

            public List<AttributesModel> AttributesData { get; set; }

            //public ProductInfoPinLocationModel ProductInfoPinLocation { get; set; }
            //public class ProductInfoPinLocationModel
            //{
            //    [JsonProperty("latitude")]
            //    public string Latitude { get; set; }

            //    [JsonProperty("longitude")]
            //    public string Longitude { get; set; }

            //    [JsonProperty("location")]
            //    public string Location { get; set; }
            //}
        }

        public class RequestForQuotationModel
        {
            [Required(ErrorMessage ="RequestId is required")]
            public int RequestId { get; set; }
            public decimal Quantity { get; set; }
        }

        public class OrderDeliveryRequestModel
        {
            public bool BagsDirectlyFromSupplier { get; set; }
            [Required(ErrorMessage = "BagsDirectlyFromWarehouse is required")]
            public bool BagsDirectlyFromWarehouse { get; set; }
            [Required(ErrorMessage = "ExpectedShipmentId is required")]
            public int ExpectedShipmentId { get; set; }
            [Required(ErrorMessage = "CountryId is required")]
            public int CountryId { get; set; }
            [Required(ErrorMessage = "CityId is required")]
            public int CityId { get; set; }
            [Required(ErrorMessage = "AreaId is required")]
            public int AreaId { get; set; }
            public string StreetAddress { get; set; }
            public string ContactNumber { get; set; }
            [Required(ErrorMessage = "Quantity is required")]
            public decimal Quantity { get; set; }
            //public DateTime ShipmentDateUtc { get; set; }
            [Required(ErrorMessage = "AgentId is required")]
            public int AgentId { get; set; }
            public int WarehouseId { get; set; }
        }

        public class RejectModel
        {
            public int Id { get; set; }
            public string RejectedReason { get; set; }
            public string RejectedOtherReason { get; set; }
        }
        public class RequestDeliveryScedule
        {
            public int Id { get; set; }
            public int BuyerRequestId { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class UploadGrn
        {
            public int deliveryRequestFormId { get; set; }
            public string fileName { get; set; }
            public byte[] imgbytes { get; set; }
            public string notes { get; set; }

            public string AlternatePickupAddress { get; set; }
            public decimal TotalDeliveryCost { get; set; }
            public string Kilometers { get; set; }
            public string TransporterName { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }

        public class ReAssignAgent
        {
            public int Id { get; set; }
            public int agentId { get; set; }
            public string type { get; set; }
        }

        public class RejectDeliveryRequest
        {
            public int ShipmentRequestId { get; set; }
            public string RejectedReason { get; set; }
            //public string RejectedOtherReason { get; set; }
        }

        public partial class OrderSalesReturnRequestModel
        {
            public int OrderId { get; set; }
            public int SupplierId { get; set; }
            public int QuotationId { get; set; }
            public DateTime ReturnRequestDateUtc { get; set; }
            public decimal Quantity { get; set; }
            public string PickupAddress { get; set; }
            public string DropOffAddress { get; set; }
            public bool IsInventory { get; set; }
            public string ReturnReason { get; set; }
            public int AgentId { get; set; }
        }

        public partial class OrderSalesReturnRejectModel
        {
            public int returnRequestId { get; set; }
            public bool type { get; set; }
            public string rejectedReason { get; set; }
            public string rejectedOtherReason { get; set; }
        }

        public class AttributesModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class OrderSearchModel
        {
            public OrderSearchModel()
            {
                showActiveOrders = true;
                pageSize = 10;
            }

            public bool showActiveOrders { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }

            public int orderId { get; set; }
            public int industryId { get; set; }
            public int categoryId { get; set; }
            public int productId { get; set; }
            public int brandId { get; set; }
            public string otherBrand { get; set; }
            public int buyerId { get; set; }
            public int bookerId { get; set; }

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public partial class DirectOrderModel
        {
            public string Comment { get; set; }
            public ProductInfoModel ProductInfo { get; set; }
            public DeliveryInfoModel DeliveryInfo { get; set; }
            public PaymentDetailModel PaymentDetail { get; set; }
            public class ProductInfoModel
            {
                public ProductInfoModel()
                {
                    AttributesData = new List<AttributesModel>();
                }
                public int BuyerId { get; set; }
                public int IndustryId { get; set; }
                public int CategoryId { get; set; }
                public int ProductId { get; set; }
                public int BrandId { get; set; }
                public string OtherBrand { get; set; }
                public decimal Quantity { get; set; }
                public List<AttributesModel> AttributesData { get; set; }
            }
            public class DeliveryInfoModel
            {
                public DeliveryInfoModel()
                {
                    DeliverySchedule = new List<DeliveryScheduleModel>();
                }
                public PinLocationModel PinLocation { get; set; }
                public int CountryId { get; set; }
                public int StateId { get; set; }
                public int AreaId { get; set; }
                public string StreetAddress { get; set; }
                public bool InterGeography { get; set; }
                public List<DeliveryScheduleModel> DeliverySchedule { get; set; }

            }
            public class PaymentDetailModel
            {
                public PaymentDetailModel()
                {
                    SellerBids = new List<Quotations>();
                }

                public BuyerModel Buyer { get; set; }
                public IList<Quotations> SellerBids { get; set; }
            }
            public class BuyerModel
            {
                //public decimal GST { get; set; }
                //public decimal SellingPrice { get; set; }
                //public string Margin { get; set; }
                //public string Promo { get; set; }
                public int PaymentTerms { get; set; }
                public int TransactionModelId { get; set; }
            }
            public class Quotations
            {
                public int SellerId { get; set; }
                public decimal Quantity { get; set; }
                public decimal UnitPrice { get; set; }
                //Payable
                public bool IsCommission { get; set; }
                public decimal CommissionRate { get; set; }
                //public string CommissionRateType { get; set; }
                //public bool GSTIncludedInTotalAmount { get; set; }
                public decimal GSTRatePayable { get; set; }
                public decimal WHTRatePayable { get; set; }

                //Recievable
                //public decimal MarginRate { get; set; }
                //public string MarginRateType { get; set; }
                //public decimal DiscountRate { get; set; }
                //public string DiscountRateType { get; set; }
                //public decimal GSTRate_Receivable { get; set; }
                //public decimal WHTRate_Receivable { get; set; }
            }
        }
        public partial class BuyerRequestBidApproveModel
        {
            public int BuyerRequestId { get; set; }
            public List<int> QuotationId { get; set; }
            public string DeliveryAddress { get; set; }
            public string DeliveryAddress2 { get; set; }
            public BuyerRequestPinLocationModel PinLocation { get; set; }
        }

        public class DeliveryScheduleModel
        {
            public DateTime DeliveryDate { get; set; }
            public decimal Quantity { get; set; }
        }

        public partial class QuotationBidApproveModel
        {
            public QuotationBidApproveModel()
            {
                DeliverySchedules = new List<DeliveryScheduleModel>();
            }
            public int BuyerRequestId { get; set; }
            public List<int> QuotationIds { get; set; }
            public IList<DeliveryScheduleModel> DeliverySchedules { get; set; }
        }
        public partial class TijaraBuyerRequestModel
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

            public List<BookerAttributesModel> AttributesData { get; set; }
            public BuyerRequestPinLocationModel PinLocation { get; set; }

            public class BookerAttributesModel
            {
                public string Name { get; set; }
                public string Value { get; set; }
            }
        }
        public class BuyerRequestPinLocationModel
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Location { get; set; }
        }
        public class BuyerInfoAddressModel
        {
            public int CountryId { get; set; }
            public int StateId { get; set; }
            public int AreaId { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public BuyerInfoAddressPinLocationModel BuyerPinLocation { get; set; }
            public class BuyerInfoAddressPinLocationModel
            {
                [JsonProperty("latitude")]
                public string Latitude { get; set; }

                [JsonProperty("longitude")]
                public string Longitude { get; set; }

                [JsonProperty("location")]
                public string Location { get; set; }
            }
        }
        public class BuyerOrderPaymentModel
        {
            public string CustomOrderNumber { get; set; }
            public string Type { get; set; }
            public string Amount { get; set; }
            public string DateFormatted { get; set; }
            public DateTime Date { get; set; }
            public bool IsUp { get; set; }
        }
        public class BookerBuyerRegisterApiModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public string Phone { get; set; }
            public int IndustryId { get; set; }
            public int BuyerType { get; set; }

            public BookerBuyerLocationModel BookerCurrentLocation { get; set; }
            public BookerBuyerLocationModel BuyerPinLocation { get; set; }

            public class BookerBuyerLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }
        }
        public class BookerSellerRegisterApiModel
        {
            public BookerSellerRegisterApiModel()
            {
                ProductIds = new List<int>();
            }

            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public int IndustryId { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public string Phone { get; set; }
            public int SupplierType { get; set; }

            public List<int> ProductIds { get; set; }

            public BookerSupplierLocationModel BookerCurrentLocation { get; set; }
            public BookerSupplierLocationModel SellerPinLocation { get; set; }

            public class BookerSupplierLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }

            public class Suppliers
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }
        public class PinLocationModel
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Location { get; set; }
        }
        public class RaisePoAddModel
        {
            public DateTime DateOfInvoice { get; set; }
            public DateTime PaymentDueDate { get; set; }
            public string AccountNumber { get; set; }
            public string PaymentTerm { get; set; }
            public string AccountTitle { get; set; }
            public string BankName { get; set; }
            public string AddOrderPaymentMessage_Payable { get; set; }
            public decimal AddOrderPaymentQuantity_Payable { get; set; }
            public int QuotationId { get; set; }
            public decimal UnitRateWithCalculation { get; internal set; }
            public int BankDetailId { get; set; }
            public int AgentId { get; set; }
            public int orderId { get; set; }
            public int supplierId { get; set; }
            public byte[] imgBytes { get; set; }
            public string fileName { get; set; }
        }

        public class RaisePOApprovedAndRejectedModel
        {
            public int PaymentOrderId { get; set; }
            public bool Type { get; set; }
            public string Comment { get; set; }
            //public byte[] imgBytes  { get; set; }
            //public string fileName  { get; set; }
        }

        public class BuyerReceivableModel
        {
            public int OrderId { get; set; }
            public int ModeOfPaymentId { get; set; }
            public string VerficationNumber { get; set; }
            public int PictureId { get; internal set; }
            public int AddOrderPaymentAmount_Receivable { get; set; }
            public DateTime? PostDatedCheque { get; set; }
            public string AddOrderPaymentMessage_Receivable { get; set; }
            public DateTime PaymentDueDate { get; set; }
            public byte[] imgBytes { get; set; }
            public string fileName { get; set; }
        }

        public partial class RFQQuotationsModel
        {
            public int Id { get; set; }
            public int SupplierId { get; set; }
            //public int BrandId { get; set; }
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }
            public int BusinessModelId { get; set; }
            public DateTime PriceValidity { get; set; }
        }

        public class Requests
        {
            public Requests()
            {
                Data = new List<BuyerRequestData>();
            }
            public string Date { get; set; }

            public List<BuyerRequestData> Data { get; set; }
        }
        public class BuyerRequestData
        {
            public int Id { get; set; }
            public string CustomRequestNumber { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int IndustryId { get; set; }
            public string IndustryName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public string OtherBrand { get; set; }
            public int BuyerId { get; set; }
            public string BuyerName { get; set; }
            public decimal Quantity { get; set; }
            public string DeliveryAddress { get; set; }
            public int TotalQuotations { get; set; }
            public string ExpiryDate { get; set; }
            public int StatusId { get; set; }
            public string Status { get; set; }
            public object AttributesInfo { get; set; }
            public string InventoryStatus { get; set; }
            public decimal RemainingQuantity { get; set; }
            public decimal TotalRfqRemainingQty { get; set; }
            public int OrderId { get; set; }
            public string CustomOrderNumber { get; set; }
        }
        public class RequestForQuotationsModel
        {
            public RequestForQuotationsModel()
            {
                Data = new List<RequestForQuotationData>();
            }
            public string Date { get; set; }

            public List<RequestForQuotationData> Data { get; set; }
        }

        public class RequestForQuotationData
        {
            public int Id { get; set; }
            public string CustomNumber { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int IndustryId { get; set; }
            public string IndustryName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int BuyerId { get; set; }
            public string BuyerName { get; set; }
            public decimal Quantity { get; set; }
            public string DeliveryAddress { get; set; }
            public int TotalQuotations { get; set; }
            public string ExpiryDate { get; set; }
            public int StatusId { get; set; }
            public string Status { get; set; }
            public object AttributesInfo { get; set; }
        }

        public class Bids
        {
            public Bids()
            {
                Data = new List<BidsData>();
            }
            public string Date { get; set; }

            public List<BidsData> Data { get; set; }
            public class BidsData
            {
                public int Id { get; set; }
                public string CustomQuotationNumber { get; set; }
                public int BuyerRequestId { get; set; }
                public int SupplierId { get; set; }
                public string SupplierName { get; set; }
                public int ProductId { get; set; }
                public string ProductName { get; set; }
                public string Brand { get; set; }
                public int IndustryId { get; set; }
                public string IndustryName { get; set; }
                public int BrandId { get; set; }
                public string BrandName { get; set; }
                public string CategoryName { get; set; }
                public string OtherBrand { get; set; }
                public int BuyerId { get; set; }
                public string BuyerName { get; set; }
                public decimal Quantity { get; set; }
                public string DeliveryAddress { get; set; }
                public int TotalQuotations { get; set; }
                public string ExpiryDate { get; set; }
                public int StatusId { get; set; }
                public string Status { get; set; }
                //public decimal Qty { get; set; }
                public string QtyType { get; set; }
                public string UnitPrice { get; set; }
                public string BidPrice { get; set; }
                public string PriceValidity { get; set; }
                public DateTime CreatedOn { get; set; }
                public int OrderId { get; set; }
                public string CustomOrderNumber { get; set; }

            }
        }

        public class DRAndSRGenericList
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string ShipmentType { get; set; }
            public string TimeRemaining { get; set; }
            public string Priority { get; set; }
            public string Date { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public string Requester { get; set; }
            public int StatusId { get; set; }
            public string Status { get; set; }
            public int AgentId { get; set; }
            public string AgentName { get; set; }
            public string OrderType { get; set; }
            public int OrderId { get; set; }
            public string CustomeOrderNumber { get; set; }
            public int CustomerId { get; set; }
            public string FullName { get; set; }
            //public string DownloadUrl { get; set; }
            //public decimal Quantity { get; set; }

            //public string Brand { get; set; }
            //public string Shipmentdate { get; set; }
            //public string BuyerName { get; set; }
            //public string Contactnumber { get; set; }
            //public string Streetaddress { get; set; }

            //public string AreaName { get; set; }
            //public string OrderCustomNumber { get; set; }
            //public string QuotationNumber { get; set; }
            ////Only Use SaleReturn
            //public string SupplierName { get; set; }
            //public bool Inventory { get; set; }
            //public string Reason { get; set; }
            //public string Dropoffaddress { get; set; }
            //public string Pickupaddress { get; set; }
        }

        public class DrANDSrList
        {
            public DrANDSrList()
            {
                Data = new List<DRAndSRData>();
            }
            public string Date { get; set; }

            public List<DRAndSRData> Data { get; set; }

        }

        public class DRAndSRData
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string TimeRemaining { get; set; }
            public string Priority { get; set; }
            public string Date { get; set; }
            //public DateTime CreatedOnUtc { get; set; }
            public string Requester { get; set; }
            public int StatusId { get; set; }
            public string Status { get; set; }
            public string ShipmentType { get; set; }
            public string OrderType { get; set; }
            //public int AgentId { get; set; }
            //public decimal Quantity { get; set; }
            public string AgentName { get; set; }
            //public string BuyerName { get; set; }
            //public string SupplierName { get; set; }
            //public bool Inventory { get; set; }
            //public string Reason { get; set; }
            //public string Dropoffaddress { get; set; }
            //public string Pickupaddress { get; set; }
            //public string Brand { get; set; }
            //public string Shipmentdate { get; set; }
            //public string Contactnumber { get; set; }
            //public string Streetaddress { get; set; }
            //public string AreaName { get; set; }
            //public string OrderCustomNumber { get; set; }
            //public string QuotationNumber { get; set; }
            // public string DownloadUrl { get; set; }
            public int OrderId { get; set; }
            public string CustomeOrderNumber { get; set; }
            public int CustomerId { get; set; }
            public string FullName { get; set; }
        }
        public class PaymentOrders
        {
            public PaymentOrders()
            {
                Data = new List<PaymentOrdersData>();
            }
            public string Date { get; set; }

            public List<PaymentOrdersData> Data { get; set; }
            public class PaymentOrdersData
            {
                public int Id { get; set; }
                public int OrderId { get; set; }
                public string OrderCustomNumber { get; set; }
                public string CreatedOnUtc { get; set; }
                public string OrderStatus { get; set; }
                public int OrderStatusId { get; set; }
                public int ProductId { get; set; }
                public string ProductName { get; set; }
                public int BuyerId { get; set; }
                public string BuyerName { get; set; }
                public int BrandId { get; set; }
                public string BrandName { get; set; }
                public int BookerId { get; set; }
                public string BookerName { get; set; }
                public decimal Quantity { get; set; }
                public string PaymentDueDate { get; set; }
                public string Industry { get; set; }

            }
        }
        public class Orders
        {
            public Orders()
            {
                Data = new List<OrdersData>();
            }
            public string Date { get; set; }

            public List<OrdersData> Data { get; set; }

        }
        public class OrdersData
        {
            public int OrderId { get; set; }
            public string OrderCustomNumber { get; set; }
            public string CreatedOnUtc { get; set; }
            public string OrderStatus { get; set; }
            public int OrderStatusId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int BuyerId { get; set; }
            public string BuyerName { get; set; }
            public int SupplierId { get; set; }
            public string SupplierName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int BookerId { get; set; }
            public string BookerName { get; set; }
            public decimal Quantity { get; set; }
            public string PaymentDueDate { get; set; }
            public string Industry { get; set; }

        }

        public class BuyerContactUploadSignatureModel
        {
            public int OrderId { get; set; }
            public int ContactId { get; set; }
            public byte[] imgBytes { get; set; }
        }
        public class MaskAsShippedModel
        {
            public MaskAsShippedModel()
            {
                Inventories = new List<InventoryApiModel>();
            }
            public int ShipmentId { get; set; }
            public int WarehouseId { get; set; }
            public int TransporterId { get; set; }
            public int VehicleId { get; set; }
            public int CostOnZarayeId { get; set; }
            public string VehicleNumber { get; set; }
            public int RouteTypeId { get; set; }
            public string PickupAddress { get; set; }
            public decimal Laborcharges { get; set; }
            public decimal ActualShippedQuantity { get; set; }
            public string ActualShippedQuantityReason { get; set; }
            public IList<InventoryApiModel> Inventories { get; set; }
        }
        public class InventoryApiModel
        {
            public int InventoryId { get; set; }
            public decimal TotalQuantity { get; set; }
            public decimal BalanceQuantity { get; set; }
            public decimal OutboundQuantity { get; set; }
        }
        public class MaskAsDeliveredModel
        {
            public int ShipmentId { get; set; }
            public string ShipmentDeliveryAddress { get; set; }
            public int TransporterTypeId { get; set; }
            public int DeliveryTypeId { get; set; }
            public int DeliveryTimingId { get; set; }
            public int DeliveryDelayedReasonId { get; set; }
            public string DeliveryCostType { get; set; }
            public decimal FreightCharges { get; set; }
            public decimal LabourCharges { get; set; }
            public decimal DeliveryCost { get; set; }
            public int DeliveryCostTypeId { get; set; }
            public int DeliveryCostReasonId { get; set; }
            public byte[] ImageBytes { get; set; }
            public string FileName { get; set; }
            public int LabourTypeId { get; set; }
            public int WarehouseId { get; set; }
            public decimal ActualDeliveredQuantity { get; set; }
            public string ActualDeliveredQuantityReason { get; set; }
        }

        public class InventoryRateModel
        {
            public InventoryRateModel()
            {
                Inventories = new List<InventoryModel>();
            }

            public int Id { get; set; }
            public string Sku { get; set; }
            public object AttributesInfo { get; set; }
            public string Brand { get; set; }
            public decimal Quantity { get; set; }
            public string LastUpdatedDateTime { get; set; }
            public decimal Rate { get; set; }

            public IList<InventoryModel> Inventories { get; set; }

            public class InventoryModel
            {
                public int Id { get; set; }
                public string InventoryStatus { get; set; }
                public decimal Quantity { get; set; }
            }
        }

        public class InventoryAddModel
        {
            public int InventoryId { get; set; }
            public decimal Rate { get; set; }
        }
        public class InventoryBuyerRequest
        {
            public int BuyerRequestId { get; set; }
        }

        public class RateModel
        {
            public int IndustryId { get; set; }
            public int CategoryId { get; set; }
            public int ProductId { get; set; }
            public string ProductAttributeXml { get; set; }
            public int BrandId { get; set; }
            public decimal Price { get; set; }

            public List<AttributesModel> AttributesData { get; set; }
        }
    }
}