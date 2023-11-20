using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class MspInventoryMapping : BaseEntity
    {
        public int InventoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
    }
}