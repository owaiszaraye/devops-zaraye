namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a supplier product mapping
    /// </summary>
    public partial class SupplierProduct : BaseEntity
    {
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
       
    }
}
