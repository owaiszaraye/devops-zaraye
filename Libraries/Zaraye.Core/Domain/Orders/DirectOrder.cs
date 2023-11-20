namespace Zaraye.Core.Domain.Orders
{
    public partial class DirectOrder : BaseEntity
    {
        public int BuyerId { get; set; }
        public int SupplierId { get; set; }
        public int BookerId { get; set; }
        public int IndustryId { get; set; }
        public int RequestId { get; set; }
        public int RequestForQuotationId { get; set; }
        public int QuotationId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        //public string ProductAttributeXml { get; set; }
        public int BrandId { get; set; }
        //public string BrandName { get; set; }
        public decimal Quantity { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public string StreetAddress { get; set; }
        public string PinLocation_Latitude { get; set; }
        public string PinLocation_Longitude { get; set; }
        public string PinLocation_Location { get; set; }
        public bool? InterGeography { get; set; }
        //public int BuyerPaymentTerm { get; set; }
        public int TransactionModelId { get; set; }
        public int OrderTypeId { get; set; }

        public BusinessModelEnum TransactionModelEnum
        {
            get => (BusinessModelEnum)TransactionModelId;
            set => TransactionModelId = (int)value;
        }

    }
}
