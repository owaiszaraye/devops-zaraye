using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class RateGroupList : BaseEntity
    {
        public int RateId { get; set; }
        public int RateGroupId { get; set; }
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductAttributeXml { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public decimal Price { get; set; }
        public decimal? Percentage { get; set; }
    }
}