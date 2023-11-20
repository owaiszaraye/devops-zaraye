using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class FavouriteRateGroup : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int AttributeValueId { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}