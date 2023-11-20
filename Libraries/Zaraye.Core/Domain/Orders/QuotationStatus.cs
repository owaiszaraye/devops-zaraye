namespace Zaraye.Core.Domain.Orders
{
    public enum QuotationStatus
    {
        Pending = 10,
        Verified = 20,
        QuotationSelected = 30,
        QuotationUnSelected = 40,
        Approved = 50, 
        Complete = 60,
        Cancelled = 70,
        UnVerified = 80,
        Expired = 90
    }
}
