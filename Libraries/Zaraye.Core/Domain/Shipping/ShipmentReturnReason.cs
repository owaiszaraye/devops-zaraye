namespace Zaraye.Core.Domain.Shipping
{
    /// <summary>
    /// Pickup point
    /// </summary>
    public partial class ShipmentReturnReason : BaseEntity
    {
        public string Name { get; set; }

        public bool Published { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }
    }
}
