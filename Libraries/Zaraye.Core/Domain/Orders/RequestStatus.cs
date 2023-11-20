namespace Zaraye.Core.Domain.Orders
{
    public enum RequestStatus
    {
        Pending = 10,
        Verified = 20,
        Processing = 30,
        Approved = 40,
        Complete = 50,
        Cancelled = 60, 
        UnVerified = 70, 
        Expired = 80
    }
}
