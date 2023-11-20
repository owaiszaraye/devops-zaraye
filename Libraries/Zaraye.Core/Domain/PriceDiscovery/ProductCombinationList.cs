namespace Zaraye.Core.Domain.PriceDiscovery
{
    public partial class ProductCombinationList : BaseEntity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int AttributeValueId { get; set; }
        public string AttributeValue { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public decimal? LastRate { get; set; }
        public int? LastSupplierId { get; set; }
    }
}