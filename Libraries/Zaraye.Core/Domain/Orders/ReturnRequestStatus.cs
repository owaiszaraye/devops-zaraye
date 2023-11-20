namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ReturnRequestStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Approved
        /// </summary>
        Approved = 20,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 30
    }
}
