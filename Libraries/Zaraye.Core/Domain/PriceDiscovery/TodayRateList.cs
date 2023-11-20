using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class TodayRateList : BaseEntity
    {
        public int SupplierId { get; set; }
        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string AttributeXml { get; set; }
        public int CreatedById { get; set; }
        public int PublishedById { get; set; }
        public bool IncludeGst { get; set; }
        public bool IncludeFirstMile { get; set; }
        public bool Published { get; set; }

        public int AttributeValueId { get; set; }
        public string AttributeValue { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public decimal Rate { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public string PreviousRates { get; set; }
    }
}