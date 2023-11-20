namespace Zaraye.Core.Domain.Inventory
{
    public enum InventoryInboundStatusEnum
    {
        Active = 10,
        Cancelled = 20,
        Booked = 30,
        InTransit = 40,
        Physical = 50,
        Virtual = 60,
        Sold = 70
    }
}