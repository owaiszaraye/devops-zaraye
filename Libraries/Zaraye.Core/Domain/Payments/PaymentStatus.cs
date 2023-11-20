namespace Zaraye.Core.Domain.Payments
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// PartiallyPaid
        /// </summary>
        PartiallyPaid = 20,

        /// <summary>
        /// Paid
        /// </summary>
        Paid = 30
    }
}
