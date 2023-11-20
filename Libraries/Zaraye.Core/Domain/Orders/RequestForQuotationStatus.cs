namespace Zaraye.Core.Domain.Orders
{
    public enum RequestForQuotationStatus
    {
        Pending = 10,
        Verified = 20,
        Processing = 30,
        Complete = 40,
        Cancelled = 50, 
        UnVerified = 60, 
        Expired = 70,
        Approved = 80
    }
}
