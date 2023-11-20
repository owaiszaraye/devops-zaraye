using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class RateGroup : BaseEntity
    {
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductAttributeXml { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}