namespace Zaraye.Core.Domain.Inventory
{
    public partial class InventoryGroup : BaseEntity, StoreEntity
    {
        public int ProductId { get; set; }
        public string ProductAttributesXml { get; set; }
        public int BrandId { get; set; }
        public int LastInventoryId { get; set; }
        public int StoreId { get; set; }
    }
}
