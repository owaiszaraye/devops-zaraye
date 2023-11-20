namespace Zaraye.Core.Domain.Inventory
{
    public partial class CogsInventoryTagging : BaseEntity
    {
        public int RequestId { get; set; }
        public int InventoryId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossQuantity { get; set; }
    }
}
